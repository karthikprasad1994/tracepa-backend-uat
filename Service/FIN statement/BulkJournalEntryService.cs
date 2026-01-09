// Services/BulkJournalEntryService.cs
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using JournalEntryUploadAPI.Services;
using JournalEntryUploadAPI.Models.DTOs;
using Dapper;
using TracePca.Interface.FIN_Statement;

namespace JournalEntryUploadAPI.Services
{
    public class BulkJournalEntryService : IBulkJournalEntryService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BulkJournalEntryService> _logger;
        private readonly Dictionary<string, int> _accountCache = new();
        private readonly Dictionary<string, int> _voucherTypeCache = new();


        public BulkJournalEntryService(IConfiguration configuration, ILogger<BulkJournalEntryService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

            

            public async Task<UploadResponseDto> BulkUploadAsync(IFormFile file, JournalEntryUploadDto request)
            {
                var response = new UploadResponseDto
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    StartTime = DateTime.UtcNow
                };

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    _logger.LogInformation($"Bulk upload started: {file.FileName}");

                    // Read Excel file
                    var journalEntries = await ReadJournalEntriesAsync(file);
                    response.TotalRecords = journalEntries.Count;

                    // Validate entries
                    var validationResult = ValidateJournalEntries(journalEntries);
                    if (!validationResult.IsValid)
                    {
                        response.Errors.AddRange(validationResult.Errors);
                        response.FailedRecords = response.TotalRecords;
                        response.Success = false;
                        response.Message = "Validation failed";
                        return response;
                    }

                    // Process in batches
                    const int batchSize = 10000;
                    int totalProcessed = 0;
                    int totalFailed = 0;

                    for (int i = 0; i < journalEntries.Count; i += batchSize)
                    {
                        var batch = journalEntries.Skip(i).Take(batchSize).ToList();
                        var batchResult = await ProcessBatchAsync(batch, request);

                        totalProcessed += batchResult.ProcessedRecords;
                        totalFailed += batchResult.FailedRecords;

                        if (batchResult.Errors.Any())
                            response.Errors.AddRange(batchResult.Errors);
                    }

                    response.ProcessedRecords = totalProcessed;
                    response.FailedRecords = totalFailed;
                    response.Success = totalFailed == 0;

                    stopwatch.Stop();
                    response.ProcessingTimeInSeconds = stopwatch.Elapsed.TotalSeconds;
                    response.RecordsPerSecond = totalProcessed / stopwatch.Elapsed.TotalSeconds;
                    response.EndTime = DateTime.UtcNow;
                    response.Message = response.Success ?
                        $"Successfully processed {totalProcessed} records in {response.ProcessingTimeInSeconds:F2} seconds" :
                        $"Processed {totalProcessed} records, {totalFailed} failed in {response.ProcessingTimeInSeconds:F2} seconds";

                    _logger.LogInformation($"Bulk upload completed: {response.Message}");

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Bulk upload failed");
                    response.Success = false;
                    response.Errors.Add($"Upload failed: {ex.Message}");
                    return response;
                }
            }

            private ValidationResult ValidateJournalEntries(List<JournalEntryDto> entries)
            {
                var result = new ValidationResult();

                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var rowNumber = i + 2; // +2 for 1-based index and header row

                    if (string.IsNullOrWhiteSpace(entry.Account))
                        result.Errors.Add($"Row {rowNumber}: Account is required");

                    if (string.IsNullOrWhiteSpace(entry.Type))
                        result.Errors.Add($"Row {rowNumber}: Voucher Type is required");

                    if (entry.Debit < 0 || entry.Credit < 0)
                        result.Errors.Add($"Row {rowNumber}: Debit/Credit amounts cannot be negative");

                    if (entry.Debit == 0 && entry.Credit == 0)
                        result.Errors.Add($"Row {rowNumber}: Either Debit or Credit must be greater than 0");
                }

                result.IsValid = !result.Errors.Any();
                return result;
            }

            private async Task<BatchResult> ProcessBatchAsync(
                List<JournalEntryDto> batch,
                JournalEntryUploadDto request)
            {
                var result = new BatchResult();

                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    // Start transaction
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Get or create accounts first
                            var accountMap = await GetOrCreateAccountsAsync(
                                batch, request, connection, transaction);

                            // Get voucher type IDs
                            var voucherTypeMap = await GetVoucherTypeIdsAsync(
                                batch, request.AccessCodeId, connection, transaction);

                            // Generate transaction numbers
                            var maxTransactionNo = await GetMaxTransactionNoAsync(
                                request, connection, transaction);

                            // Prepare data tables
                            var masterTable = CreateMasterDataTable();
                            var detailTable = CreateDetailDataTable();
                            var trailBalanceTable = CreateTrailBalanceDataTable();

                            // Populate tables
                            int transactionCounter = 1;
                            foreach (var entry in batch)
                            {
                                if (!accountMap.TryGetValue(entry.Account, out int accountId) ||
                                    !voucherTypeMap.TryGetValue(entry.Type, out int voucherTypeId))
                                {
                                    result.FailedRecords++;
                                    result.Errors.Add($"Skipped entry: Account '{entry.Account}' or Voucher Type '{entry.Type}' not found");
                                    continue;
                                }

                                var transactionNo = $"JE00-{maxTransactionNo + transactionCounter}";
                                var journalEntryId = await InsertMasterRecordAsync(
                                    entry, request, transactionNo, voucherTypeId,
                                    connection, transaction);

                                if (journalEntryId > 0)
                                {
                                    // Insert detail records
                                    if (entry.Debit > 0)
                                    {
                                        await InsertDetailRecordAsync(
                                            entry, request, transactionNo, accountId, voucherTypeId,
                                            entry.Debit, 0, journalEntryId, connection, transaction);
                                    }

                                    if (entry.Credit > 0)
                                    {
                                        await InsertDetailRecordAsync(
                                            entry, request, transactionNo, accountId, voucherTypeId,
                                            0, entry.Credit, journalEntryId, connection, transaction);
                                    }

                                    // Update trail balance
                                    await UpdateTrailBalanceAsync(
                                        accountId, request, entry.Debit, entry.Credit,
                                        connection, transaction);

                                    result.ProcessedRecords++;
                                }
                                else
                                {
                                    result.FailedRecords++;
                                }

                                transactionCounter++;
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            result.FailedRecords = batch.Count;
                            result.Errors.Add($"Batch processing failed: {ex.Message}");
                            _logger.LogError(ex, "Batch processing failed");
                        }
                    }
                }

                return result;
            }

            private async Task<Dictionary<string, int>> GetOrCreateAccountsAsync(
                List<JournalEntryDto> entries,
                JournalEntryUploadDto request,
                SqlConnection connection,
                SqlTransaction transaction)
            {
                var accountMap = new Dictionary<string, int>();
                var distinctAccounts = entries.Select(e => e.Account).Distinct();

                // Get existing accounts
                var existingQuery = @"
            SELECT ATBU_ID, ATBU_Description 
            FROM Acc_TrailBalance_Upload 
            WHERE ATBU_CustId = @CustomerId 
              AND ATBU_Branchid = @BranchId 
              AND ATBU_CompId = @AccessCodeId 
              AND ATBU_YEARId = @FinancialYearId 
              AND ATBU_QuarterId = @DurationId 
              AND ATBU_Description IN ({0})";

                var paramNames = distinctAccounts.Select((_, i) => $"@Account{i}").ToArray();
                var paramValues = distinctAccounts.Select((a, i) => new SqlParameter($"@Account{i}", a)).ToArray();

                existingQuery = string.Format(existingQuery, string.Join(",", paramNames));

                using (var command = new SqlCommand(existingQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                    command.Parameters.AddWithValue("@BranchId", request.BranchId);
                    command.Parameters.AddWithValue("@AccessCodeId", request.AccessCodeId);
                    command.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                    command.Parameters.AddWithValue("@DurationId", request.DurationId);
                    command.Parameters.AddRange(paramValues);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            accountMap[reader.GetString(1)] = reader.GetInt32(0);
                        }
                    }
                }

                // Create missing accounts
                var missingAccounts = distinctAccounts.Where(a => !accountMap.ContainsKey(a));
                foreach (var account in missingAccounts)
                {
                    var accountId = await CreateAccountAsync(account, request, connection, transaction);
                    if (accountId > 0)
                        accountMap[account] = accountId;
                }

                return accountMap;
            }

            private async Task<int> CreateAccountAsync(
                string accountName,
                JournalEntryUploadDto request,
                SqlConnection connection,
                SqlTransaction transaction)
            {
                // Get next ATBU_ID
                var maxIdQuery = "SELECT ISNULL(MAX(ATBU_ID), 0) FROM Acc_TrailBalance_Upload WITH (UPDLOCK, HOLDLOCK)";
                int nextId;

                using (var command = new SqlCommand(maxIdQuery, connection, transaction))
                {
                    nextId = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                }

                // Get account count for code generation
                var countQuery = @"
            SELECT COUNT(*) 
            FROM Acc_TrailBalance_Upload 
            WHERE ATBU_CustId = @CustomerId 
              AND ATBU_CompId = @AccessCodeId 
              AND ATBU_YEARId = @FinancialYearId 
              AND ATBU_Branchid = @BranchId 
              AND ATBU_QuarterId = @DurationId";

                int accountCount;
                using (var command = new SqlCommand(countQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                    command.Parameters.AddWithValue("@AccessCodeId", request.AccessCodeId);
                    command.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                    command.Parameters.AddWithValue("@BranchId", request.BranchId);
                    command.Parameters.AddWithValue("@DurationId", request.DurationId);

                    accountCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                var accountCode = $"SCh00{accountCount + 1}";

                // Insert master account
                var insertMasterQuery = @"
            INSERT INTO Acc_TrailBalance_Upload (
                ATBU_ID, ATBU_CODE, ATBU_Description,
                ATBU_CustId, ATBU_Branchid,
                ATBU_Opening_Debit_Amount, ATBU_Opening_Credit_Amount,
                ATBU_TR_Debit_Amount, ATBU_TR_Credit_Amount,
                ATBU_Closing_Debit_Amount, ATBU_Closing_Credit_Amount,
                ATBU_DELFLG, ATBU_CRBY, ATBU_STATUS, ATBU_UPDATEDBY,
                ATBU_IPAddress, ATBU_CompId, ATBU_YEARId, ATBU_QuarterId,
                ATBU_CRON, ATBU_UPDATEDON,
                ATBU_Closing_TotalDebit_Amount, ATBU_Closing_TotalCredit_Amount,
                ATBU_Progress
            ) VALUES (
                @ATBU_ID, @ATBU_CODE, @ATBU_Description,
                @ATBU_CustId, @ATBU_Branchid,
                0, 0, 0, 0, 0, 0,
                'A', @ATBU_CRBY, 'C', @ATBU_CRBY,
                @ATBU_IPAddress, @ATBU_CompId, @ATBU_YEARId, @ATBU_QuarterId,
                GETDATE(), GETDATE(),
                0, 0, 'Uploaded'
            )";

                using (var command = new SqlCommand(insertMasterQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@ATBU_ID", nextId);
                    command.Parameters.AddWithValue("@ATBU_CODE", accountCode);
                    command.Parameters.AddWithValue("@ATBU_Description", accountName);
                    command.Parameters.AddWithValue("@ATBU_CustId", request.CustomerId);
                    command.Parameters.AddWithValue("@ATBU_Branchid", request.BranchId);
                    command.Parameters.AddWithValue("@ATBU_CRBY", request.UserId);
                    command.Parameters.AddWithValue("@ATBU_IPAddress", request.IpAddress);
                    command.Parameters.AddWithValue("@ATBU_CompId", request.AccessCodeId);
                    command.Parameters.AddWithValue("@ATBU_YEARId", request.FinancialYearId);
                    command.Parameters.AddWithValue("@ATBU_QuarterId", request.DurationId);

                    await command.ExecuteNonQueryAsync();
                }

                // Insert detail record
                var nextDetailId = await GetNextDetailIdAsync(connection, transaction);

                var insertDetailQuery = @"
            INSERT INTO Acc_TrailBalance_Upload_Details (
                ATBUD_ID, ATBUD_Masid, ATBUD_CODE,
                ATBUD_Description, ATBUD_CustId,
                Atbud_Branchnameid, ATBUd_QuarterId,
                ATBUD_Company_Type, ATBUD_Headingid,
                ATBUD_Subheading, ATBUD_itemid,
                ATBUD_Subitemid, ATBUD_DELFLG,
                ATBUD_CRBY, ATBUD_UPDATEDBY,
                ATBUD_STATUS, ATBUD_Progress,
                ATBUD_IPAddress, ATBUD_CompId,
                ATBUD_YEARId, ATBUD_CRON,
                ATBUD_UPDATEDON
            ) VALUES (
                @ATBUD_ID, @ATBUD_Masid, @ATBUD_CODE,
                @ATBUD_Description, @ATBUD_CustId,
                @Atbud_Branchnameid, @ATBUd_QuarterId,
                @ATBUD_Company_Type, 0, 0, 0, 0,
                'A', @ATBUD_CRBY, @ATBUD_CRBY,
                'C', 'Uploaded',
                @ATBUD_IPAddress, @ATBUD_CompId,
                @ATBUD_YEARId, GETDATE(), GETDATE()
            )";

                using (var command = new SqlCommand(insertDetailQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@ATBUD_ID", nextDetailId);
                    command.Parameters.AddWithValue("@ATBUD_Masid", nextId);
                    command.Parameters.AddWithValue("@ATBUD_CODE", accountCode);
                    command.Parameters.AddWithValue("@ATBUD_Description", accountName);
                    command.Parameters.AddWithValue("@ATBUD_CustId", request.CustomerId);
                    command.Parameters.AddWithValue("@Atbud_Branchnameid", request.BranchId);
                    command.Parameters.AddWithValue("@ATBUd_QuarterId", request.DurationId);
                    command.Parameters.AddWithValue("@ATBUD_Company_Type", request.CustomerId);
                    command.Parameters.AddWithValue("@ATBUD_CRBY", request.UserId);
                    command.Parameters.AddWithValue("@ATBUD_IPAddress", request.IpAddress);
                    command.Parameters.AddWithValue("@ATBUD_CompId", request.AccessCodeId);
                    command.Parameters.AddWithValue("@ATBUD_YEARId", request.FinancialYearId);

                    await command.ExecuteNonQueryAsync();
                }

                return nextId;
            }

            private async Task<Dictionary<string, int>> GetVoucherTypeIdsAsync(
                List<JournalEntryDto> entries,
                int accessCodeId,
                SqlConnection connection,
                SqlTransaction transaction)
            {
                var voucherTypeMap = new Dictionary<string, int>();
                var distinctTypes = entries.Select(e => e.Type).Distinct();

                var query = @"
            SELECT cmm_ID, cmm_Desc 
            FROM Content_Management_Master 
            WHERE CMM_CompID = @AccessCodeId 
              AND cmm_Category = 'JE' 
              AND cmm_Delflag = 'A' 
              AND cmm_Desc IN ({0})";

                var paramNames = distinctTypes.Select((_, i) => $"@Type{i}").ToArray();
                var paramValues = distinctTypes.Select((t, i) => new SqlParameter($"@Type{i}", t)).ToArray();

                query = string.Format(query, string.Join(",", paramNames));

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@AccessCodeId", accessCodeId);
                    command.Parameters.AddRange(paramValues);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            voucherTypeMap[reader.GetString(1)] = reader.GetInt32(0);
                        }
                    }
                }

                return voucherTypeMap;
            }

            private async Task<int> GetMaxTransactionNoAsync(
                JournalEntryUploadDto request,
                SqlConnection connection,
                SqlTransaction transaction)
            {
                var query = @"
            SELECT ISNULL(MAX(CAST(SUBSTRING(Acc_JE_TransactionNo, 6, LEN(Acc_JE_TransactionNo)) AS INT)), 0)
            FROM Acc_JE_Master 
            WHERE Acc_JE_YearID = @FinancialYearId 
              AND Acc_JE_Party = @CustomerId 
              AND acc_JE_BranchId = @BranchId 
              AND Acc_JE_QuarterId = @DurationId
              AND Acc_JE_TransactionNo LIKE 'JE00-%'";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                    command.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                    command.Parameters.AddWithValue("@BranchId", request.BranchId);
                    command.Parameters.AddWithValue("@DurationId", request.DurationId);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }

            private async Task<int> InsertMasterRecordAsync(
                JournalEntryDto entry,
                JournalEntryUploadDto request,
                string transactionNo,
                int voucherTypeId,
                SqlConnection connection,
                SqlTransaction transaction)
            {
                var query = @"
            INSERT INTO Acc_JE_Master (
                Acc_JE_TransactionNo, Acc_JE_Party, Acc_JE_BillType, 
                Acc_JE_BillNo, Acc_JE_BillDate, Acc_JE_BillAmount, 
                Acc_JE_YearID, Acc_JE_CompID, Acc_JE_CreatedBy, 
                Acc_JE_Status, acc_JE_BranchId, Acc_JE_QuarterId, 
                Acc_JE_Comnments, Acc_JE_CreatedOn, Acc_JE_IPAddress
            ) 
            OUTPUT INSERTED.Acc_JE_ID
            VALUES (
                @TransactionNo, @CustomerId, @VoucherTypeId, 
                @BillNo, @BillDate, @BillAmount, 
                @FinancialYearId, @AccessCodeId, @UserId, 
                'A', @BranchId, @DurationId, @Comments, 
                GETDATE(), @IpAddress
            )";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@TransactionNo", transactionNo);
                    command.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                    command.Parameters.AddWithValue("@VoucherTypeId", voucherTypeId);
                    command.Parameters.AddWithValue("@BillNo", entry.Trans ?? "");
                    command.Parameters.AddWithValue("@BillDate", entry.Date);
                    command.Parameters.AddWithValue("@BillAmount", entry.Debit > 0 ? entry.Debit : entry.Credit);
                    command.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                    command.Parameters.AddWithValue("@AccessCodeId", request.AccessCodeId);
                    command.Parameters.AddWithValue("@UserId", request.UserId);
                    command.Parameters.AddWithValue("@BranchId", request.BranchId);
                    command.Parameters.AddWithValue("@DurationId", request.DurationId);
                    command.Parameters.AddWithValue("@Comments", entry.Memo ?? "");
                    command.Parameters.AddWithValue("@IpAddress", request.IpAddress);

                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }

            private async Task InsertDetailRecordAsync(
                JournalEntryDto entry,
                JournalEntryUploadDto request,
                string transactionNo,
                int accountId,
                int voucherTypeId,
                decimal debit,
                decimal credit,
                int journalEntryId,
                SqlConnection connection,
                SqlTransaction transaction)
            {
                var query = @"
            INSERT INTO Acc_JETransactions_Details (
                AJTB_TranscNo, AJTB_CustId, AJTB_BranchId, AJTB_Deschead, AJTB_Desc, 
                AJTB_DescName, AJTB_Debit, AJTB_Credit, AJTB_CreatedBy, AJTB_CreatedOn,
                AJTB_Status, AJTB_IPAddress, AJTB_CompID, AJTB_YearID, AJTB_BillType,
                AJTB_QuarterId
            ) VALUES (
                @TransactionNo, @CustomerId, @BranchId, @AccountId, @AccountId,
                @AccountName, @Debit, @Credit, @UserId, GETDATE(),
                'A', @IpAddress, @AccessCodeId, @FinancialYearId, @VoucherTypeId,
                @DurationId
            )";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@TransactionNo", transactionNo);
                    command.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                    command.Parameters.AddWithValue("@BranchId", request.BranchId);
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    command.Parameters.AddWithValue("@AccountName", entry.Account);
                    command.Parameters.AddWithValue("@Debit", debit);
                    command.Parameters.AddWithValue("@Credit", credit);
                    command.Parameters.AddWithValue("@UserId", request.UserId);
                    command.Parameters.AddWithValue("@IpAddress", request.IpAddress);
                    command.Parameters.AddWithValue("@AccessCodeId", request.AccessCodeId);
                    command.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                    command.Parameters.AddWithValue("@VoucherTypeId", voucherTypeId);
                    command.Parameters.AddWithValue("@DurationId", request.DurationId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            private async Task UpdateTrailBalanceAsync(
                int accountId,
                JournalEntryUploadDto request,
                decimal debit,
                decimal credit,
                SqlConnection connection,
                SqlTransaction transaction)
            {
                var query = @"
            UPDATE Acc_TrailBalance_Upload 
            SET 
                ATBU_Closing_TotalDebit_Amount = ISNULL(ATBU_Closing_TotalDebit_Amount, 0) + @DebitAmount,
                ATBU_Closing_TotalCredit_Amount = ISNULL(ATBU_Closing_TotalCredit_Amount, 0) + @CreditAmount,
                ATBU_UPDATEDBY = @CustomerId,
                ATBU_UPDATEDON = GETDATE()
            WHERE ATBU_ID = @AccountId 
              AND ATBU_CustId = @CustomerId 
              AND ATBU_CompID = @AccessCodeId";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    command.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                    command.Parameters.AddWithValue("@AccessCodeId", request.AccessCodeId);
                    command.Parameters.AddWithValue("@DebitAmount", debit);
                    command.Parameters.AddWithValue("@CreditAmount", credit);

                    await command.ExecuteNonQueryAsync();
                }
            }

            private async Task<int> GetNextDetailIdAsync(SqlConnection connection, SqlTransaction transaction)
            {
                var query = "SELECT ISNULL(MAX(ATBUD_ID), 0) + 1 FROM Acc_TrailBalance_Upload_Details WITH (UPDLOCK, HOLDLOCK)";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }

            private DataTable CreateMasterDataTable()
            {
                var table = new DataTable();
                table.Columns.Add("Acc_JE_ID", typeof(int));
                table.Columns.Add("Acc_JE_TransactionNo", typeof(string));
                table.Columns.Add("Acc_JE_Party", typeof(int));
                table.Columns.Add("Acc_JE_BillType", typeof(int));
                table.Columns.Add("Acc_JE_BillNo", typeof(string));
                table.Columns.Add("Acc_JE_BillDate", typeof(DateTime));
                table.Columns.Add("Acc_JE_BillAmount", typeof(decimal));
                table.Columns.Add("Acc_JE_YearID", typeof(int));
                table.Columns.Add("Acc_JE_CompID", typeof(int));
                table.Columns.Add("Acc_JE_CreatedBy", typeof(int));
                table.Columns.Add("Acc_JE_Status", typeof(string));
                table.Columns.Add("acc_JE_BranchId", typeof(int));
                table.Columns.Add("Acc_JE_QuarterId", typeof(int));
                table.Columns.Add("Acc_JE_Comnments", typeof(string));
                table.Columns.Add("Acc_JE_CreatedOn", typeof(DateTime));
                table.Columns.Add("Acc_JE_IPAddress", typeof(string));
                return table;
            }

            private DataTable CreateDetailDataTable()
            {
                var table = new DataTable();
                table.Columns.Add("AJTB_Id", typeof(int));
                table.Columns.Add("AJTB_TranscNo", typeof(string));
                table.Columns.Add("AJTB_CustId", typeof(int));
                table.Columns.Add("AJTB_BranchId", typeof(int));
                table.Columns.Add("AJTB_Deschead", typeof(int));
                table.Columns.Add("AJTB_Desc", typeof(int));
                table.Columns.Add("AJTB_DescName", typeof(string));
                table.Columns.Add("AJTB_Debit", typeof(decimal));
                table.Columns.Add("AJTB_Credit", typeof(decimal));
                table.Columns.Add("AJTB_CreatedBy", typeof(int));
                table.Columns.Add("AJTB_CreatedOn", typeof(DateTime));
                table.Columns.Add("AJTB_Status", typeof(string));
                table.Columns.Add("AJTB_IPAddress", typeof(string));
                table.Columns.Add("AJTB_CompID", typeof(int));
                table.Columns.Add("AJTB_YearID", typeof(int));
                table.Columns.Add("AJTB_BillType", typeof(int));
                table.Columns.Add("AJTB_QuarterId", typeof(int));
                return table;
            }

            private DataTable CreateTrailBalanceDataTable()
            {
                var table = new DataTable();
                table.Columns.Add("ATBU_ID", typeof(int));
                table.Columns.Add("DebitAmount", typeof(decimal));
                table.Columns.Add("CreditAmount", typeof(decimal));
                table.Columns.Add("CustomerId", typeof(int));
                table.Columns.Add("AccessCodeId", typeof(int));
                return table;
            }

            private class ValidationResult
            {
                public bool IsValid { get; set; }
                public List<string> Errors { get; set; } = new List<string>();
            }

            private class BatchResult
            {
                public int ProcessedRecords { get; set; }
                public int FailedRecords { get; set; }
                public List<string> Errors { get; set; } = new List<string>();
            }
        
        public async Task<List<JournalEntryDto>> ReadJournalEntriesAsync(IFormFile file)
        {
            var journalEntries = new List<JournalEntryDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    // Start from row 2 (assuming row 1 is header)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var entry = new JournalEntryDto
                            {
                                SrNo = worksheet.Cells[row, 1]?.Text?.Trim(),
                                Trans = worksheet.Cells[row, 2]?.Text?.Trim(),
                                Type = worksheet.Cells[row, 3]?.Text?.Trim(),
                                Num = worksheet.Cells[row, 5]?.Text?.Trim(),
                                Adj = worksheet.Cells[row, 6]?.Text?.Trim(),
                                Name = worksheet.Cells[row, 7]?.Text?.Trim(),
                                Memo = worksheet.Cells[row, 8]?.Text?.Trim(),
                                Account = worksheet.Cells[row, 9]?.Text?.Trim(),
                            };

                            // Parse date
                            if (DateTime.TryParse(worksheet.Cells[row, 4]?.Text, out DateTime date))
                                entry.Date = date;
                            else
                                entry.Date = DateTime.Today;

                            // Parse amounts
                            decimal.TryParse(worksheet.Cells[row, 10]?.Text, out decimal debit);
                            decimal.TryParse(worksheet.Cells[row, 11]?.Text, out decimal credit);
                            entry.Debit = debit;
                            entry.Credit = credit;

                            journalEntries.Add(entry);
                        }
                        catch (Exception ex)
                        {
                            // Log or handle individual row errors
                            continue;
                        }
                    }
                }
            }

            return journalEntries;
        }
    }
}