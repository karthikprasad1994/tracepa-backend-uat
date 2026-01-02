using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using TracePca.Interface;
using TracePca.Models;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static Dropbox.Api.TeamLog.GroupJoinPolicy;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;
using static TracePca.Service.FIN_statement.JournalEntryService;
using static TracePca.Service.FIN_statement.BulkOperationsService;



namespace TracePca.Service.FIN_statement
{
    public class JournalEntryService : JournalEntryInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BulkOperationsService _bulkService;

        public JournalEntryService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, BulkOperationsService bulkService)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _bulkService = bulkService;

        }

        //GetJournalEntryInformation
        public async Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(
         int CompId, int UserId, string Status, int CustId, int YearId, int BranchId, string DateFormat, int DurationId)
        {
            // Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Status filter mapping
            var statusFilter = Status switch
            {
                "0" => "A",
                "1" => "D",
                "2" => "W",
                _ => null
            };

            // SINGLE optimized SQL query (master + detail + grouping)
            var sql = @"
        SELECT 
            M.Acc_JE_ID AS Id,
            M.Acc_JE_TransactionNo AS TransactionNo,
            M.acc_JE_BranchId AS BranchID,
            '' AS BillNo,
            M.Acc_JE_BillDate AS BillDate,
            A.cmm_Desc AS BillType,
            M.Acc_JE_Party AS PartyID,
            M.Acc_JE_Status AS Status,
            M.Acc_JE_Comnments AS Comments,
            M.acc_JE_QuarterId,
            SUM(D.AJTB_Debit) AS Debit,
            SUM(D.AJTB_Credit) AS Credit,
            STRING_AGG(CASE WHEN D.AJTB_Debit > 0 THEN D.AJTB_DescName END, ', ') AS DebDescription,
            STRING_AGG(CASE WHEN D.AJTB_Credit > 0 THEN D.AJTB_DescName END, ', ') AS CredDescription
        FROM Acc_JE_Master M
        LEFT JOIN content_management_master A ON A.cmm_ID = M.Acc_JE_BillType
        LEFT JOIN Acc_JETransactions_Details D 
            ON D.Ajtb_Masid = M.Acc_JE_ID 
            AND D.AJTB_CustId = @CustId
        WHERE M.Acc_JE_Party = @CustId
            AND M.Acc_JE_CompID = @CompId
            AND M.Acc_JE_YearId = @YearId
            AND (@statusFilter IS NULL OR M.Acc_JE_Status = @statusFilter)
            AND (@BranchId = 0 OR M.acc_JE_BranchID = @BranchId)
            AND (@DurationId = 0 OR M.Acc_JE_QuarterId = @DurationId)
        GROUP BY 
            M.Acc_JE_ID, M.Acc_JE_TransactionNo, M.acc_JE_BranchId, 
            M.Acc_JE_BillDate, A.cmm_Desc, M.Acc_JE_Party, M.Acc_JE_Status,
            M.Acc_JE_Comnments, M.acc_JE_QuarterId
        ORDER BY M.Acc_JE_ID ASC";

            var entries = (await connection.QueryAsync<JournalEntryInformationDto>(
                sql,
                new { CompId, CustId, YearId, BranchId, DurationId, statusFilter }
            )).ToList();

            // Map status names
            foreach (var entry in entries)
            {
                entry.Status = entry.Status switch
                {
                    "W" => "Waiting For Approval",
                    "A" => "Activated",
                    "D" => "De-Activated",
                    _ => ""
                };
            }

            return entries;
        }
        public async Task<int?> GetContentMasterIdAsync(string description, int compId)
        {
            int? result = null;

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT TOP 1 cmm_ID
            FROM Content_Management_Master
            WHERE cmm_Desc = @Desc
              AND CMM_CompID = @CompId
              AND cmm_Delflag = 'A'";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Desc", description);
            cmd.Parameters.AddWithValue("@CompId", compId);

            var scalar = await cmd.ExecuteScalarAsync();
            if (scalar != null && scalar != DBNull.Value)
                result = Convert.ToInt32(scalar);

            return result;
        }

        //GetExistingJournalVouchers
        public async Task<IEnumerable<JournalEntryVoucherDto>> LoadExistingVoucherNosAsync(int compId, int yearId, int partyId, int branchId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT Acc_JE_TransactionNo, Acc_JE_ID
        FROM Acc_JE_Master
        WHERE Acc_JE_CompID = @CompId
          AND Acc_JE_YearID = @YearId
          AND Acc_JE_Party = @PartyId";

            // ✅ Add branch filter if provided
            if (branchId != 0)
            {
                query += " AND acc_je_BranchID = @BranchId";
            }

            return await connection.QueryAsync<JournalEntryVoucherDto>(query, new{CompId = compId, YearId = yearId, PartyId = partyId, BranchId = branchId});
        }

        //GetJEType 
        public async Task<IEnumerable<GeneralMasterJETypeDto>> LoadGeneralMastersAsync(int compId, string type)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT cmm_ID, cmm_Desc
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompId
          AND cmm_Category = @Type
          AND cmm_Delflag = 'A'
        ORDER BY cmm_Desc ASC";

            return await connection.QueryAsync<GeneralMasterJETypeDto>(query, new{ CompId = compId, Type = type});
        }


        //GetHeadOfAccounts
        public async Task<IEnumerable<DescheadDto>> LoadDescheadAsync(int compId, int custId, int yearId, int branchId, int durationId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            ATBU_ID, 
            ATBU_Description 
        FROM Acc_TrailBalance_Upload 
        WHERE 
            ATBU_CustId = @CustId AND 
            ATBU_CompId = @CompId AND 
            ATBU_YEARId = @YearId AND 
            ATBU_Branchid = @BranchId AND 
            ATBU_QuarterId = @DurationId";
            
            return await connection.QueryAsync<DescheadDto>(query, new
            {
                CompId = compId,
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });
        }

        //GetGeneralLedger 
        public async Task<IEnumerable<SubGlDto>> LoadSubGLDetailsAsync(int compId, int custId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            CC_GL AS gl_id, 
            CC_GLCode + '-' + CC_Gldesc AS GlDesc 
        FROM Customer_coa 
        WHERE 
            CC_compid = @CompId 
            AND CC_head = 3 
            AND CC_CustId = @CustId 
        ORDER BY CC_gl";

            return await connection.QueryAsync<SubGlDto>(query, new

            {
                CompId = compId,
                CustId = custId
            });
        }

        //SaveOrUpdateTransactionDetails
        public async Task<int[]> SaveJournalEntryWithTransactionsAsync(List<SaveJournalEntryWithTransactionsDto> dtos)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            string Transaction;
            int updateOrSave = 0, oper = 0;
            try
            {
                foreach (var dto in dtos)
                {
                    dto.Acc_JE_TransactionNo = await GenerateTransactionNoAsync(dto.Acc_JE_YearID, dto.Acc_JE_Party, dto.Acc_JE_QuarterId, dto.acc_JE_BranchId);
                    Transaction = dto.Acc_JE_TransactionNo;
                    int iPKId = dto.Acc_JE_ID;

                    // --- Save Journal Entry Master ---
                    using (var cmdMaster = new SqlCommand("spAcc_JE_Master", connection, transaction))
                    {
                        cmdMaster.CommandType = CommandType.StoredProcedure;

                        cmdMaster.Parameters.AddWithValue("@Acc_JE_ID", dto.Acc_JE_ID);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_TransactionNo", dto.Acc_JE_TransactionNo ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_Party", dto.Acc_JE_Party);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_Location", dto.Acc_JE_Location);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BillType", dto.Acc_JE_BillType);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BillNo", dto.Acc_JE_BillNo ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BillDate", dto.Acc_JE_BillDate);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BillAmount", dto.Acc_JE_BillAmount);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_AdvanceAmount", dto.Acc_JE_AdvanceAmount);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_AdvanceNaration", dto.Acc_JE_AdvanceNaration ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BalanceAmount", dto.Acc_JE_BalanceAmount);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_NetAmount", dto.Acc_JE_NetAmount);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_PaymentNarration", dto.Acc_JE_PaymentNarration ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_ChequeNo", dto.Acc_JE_ChequeNo ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_ChequeDate", dto.Acc_JE_ChequeDate);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_IFSCCode", dto.Acc_JE_IFSCCode ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BankName", dto.Acc_JE_BankName ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BranchName", dto.Acc_JE_BranchName ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_CreatedBy", dto.Acc_JE_CreatedBy);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_YearID", dto.Acc_JE_YearID);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_CompID", dto.Acc_JE_CompID);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_Status", dto.Acc_JE_Status ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_Operation", dto.Acc_JE_Operation ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_IPAddress", dto.Acc_JE_IPAddress ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_BillCreatedDate", dto.Acc_JE_BillCreatedDate);
                        cmdMaster.Parameters.AddWithValue("@acc_JE_BranchId", dto.acc_JE_BranchId);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_QuarterId", dto.Acc_JE_QuarterId);
                        cmdMaster.Parameters.AddWithValue("@Acc_JE_Comnments", dto.Acc_JE_Comments ?? string.Empty);

                        var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                        cmdMaster.Parameters.Add(updateOrSaveParam);
                        cmdMaster.Parameters.Add(operParam);

                        await cmdMaster.ExecuteNonQueryAsync();

                        updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        oper = (int)(operParam.Value ?? 0);
                    }

                    foreach (var t in dto.Transactions)
                    {
                        if (t.AJTB_MasID == dto.Acc_JE_ID)
                        {
                            using (var cmdDetail = new SqlCommand("spAcc_JETransactions_Details", connection, transaction))
                            {
                                cmdDetail.CommandType = CommandType.StoredProcedure;

                                cmdDetail.Parameters.AddWithValue("@AJTB_ID", t.AJTB_ID);
                                cmdDetail.Parameters.AddWithValue("@AJTB_MasID", oper);
                                cmdDetail.Parameters.AddWithValue("@AJTB_TranscNo", Transaction ?? string.Empty);
                                cmdDetail.Parameters.AddWithValue("@AJTB_CustId", t.AJTB_CustId);
                                cmdDetail.Parameters.AddWithValue("@AJTB_ScheduleTypeid", t.AJTB_ScheduleTypeid);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Deschead", t.AJTB_Deschead);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Desc", t.AJTB_Desc);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Debit", t.AJTB_Debit);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Credit", t.AJTB_Credit);
                                cmdDetail.Parameters.AddWithValue("@AJTB_CreatedBy", t.AJTB_CreatedBy);
                                cmdDetail.Parameters.AddWithValue(
                "@AJTB_CreatedOn",
                dto.Acc_JE_BillDate ?? DateTime.Now
            );

                                cmdDetail.Parameters.AddWithValue("@AJTB_UpdatedBy", t.AJTB_UpdatedBy);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Status", t.AJTB_Status ?? string.Empty);
                                cmdDetail.Parameters.AddWithValue("@AJTB_IPAddress", t.AJTB_IPAddress ?? string.Empty);
                                cmdDetail.Parameters.AddWithValue("@AJTB_CompID", t.AJTB_CompID);
                                cmdDetail.Parameters.AddWithValue("@AJTB_YearID", t.AJTB_YearID);
                                cmdDetail.Parameters.AddWithValue("@AJTB_BillType", t.AJTB_BillType);
                                cmdDetail.Parameters.AddWithValue("@AJTB_DescName", t.AJTB_DescName ?? string.Empty);
                                cmdDetail.Parameters.AddWithValue("@AJTB_BranchId", t.AJTB_BranchId);
                                cmdDetail.Parameters.AddWithValue("@AJTB_QuarterId", t.AJTB_QuarterId);

                                var updateOrSaveDetail = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                                var operDetail = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                                cmdDetail.Parameters.Add(updateOrSaveDetail);
                                cmdDetail.Parameters.Add(operDetail);

                                await cmdDetail.ExecuteNonQueryAsync();
                                if (dto.Acc_JE_Status == "A")
                                {
                                    await UpdateJeDetAsync(
                                        t.AJTB_CompID,
                                        t.AJTB_YearID,
                                        oper,                  // ✅ use new Master ID
                                        t.AJTB_CustId,
                                        (t.AJTB_Debit > 0 ? 0 : 1),  // 0=Debit, 1=Credit
                                        (t.AJTB_Debit > 0 ? t.AJTB_Debit : t.AJTB_Credit),
                                        t.AJTB_BranchId,
                                        t.AJTB_Debit,
                                        t.AJTB_Credit,
                                        t.AJTB_QuarterId,
                                        t.AJTB_Deschead,
                                        t.AJTB_Desc,
                                        t.AJTB_DescName
                                    );
                                }

                            }
                        }
                    }
                }

                transaction.Commit();
                return new int[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateJeDetAsync(
    int compId,
    int yearId,
    int id,                  // JE Master ID
    int custId,
    int transId,             // 0 = Debit, 1 = Credit
    decimal transAmt,        // Net Transaction Amount
    int branchId,
    decimal transDbAmt,      // Transaction Debit
    decimal transCrAmt,      // Transaction Credit
    int durtnId,             // Quarter Id
    int deschead,            // ATBU_ID → goes to AJTB_Deschead
    int descId,              // Duplicate of ATBU_ID → goes to AJTB_Desc
    string descName  )        // ATBU_Description → goes to AJTB_DescName

        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // 🔹 Select the Trail Balance row for given parameters
            string sql = @"
        SELECT TOP 1 *
        FROM Acc_TrailBalance_Upload
        WHERE ATBU_CustId = @CustId
          AND ATBU_CompID = @CompId
          AND ATBU_QuarterId = @DurtnId
          AND ATBU_ID = @Deschead
          AND ATBU_BranchId = @BranchId";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new
            {
                CustId = custId,
                CompId = compId,
                DurtnId = durtnId,
                BranchId = branchId,
                Deschead = deschead
            });

            if (row == null) return;

            // 🔹 Safe cast of values
            decimal debitAmt = (decimal?)row.ATBU_Closing_TotalDebit_Amount ?? 0m;
            decimal creditAmt = (decimal?)row.ATBU_Closing_TotalCredit_Amount ?? 0m;

            string updateSql = string.Empty;

            // 🔹 Debit Transaction (transId = 0)
            if (transId == 0)
            {
                if (debitAmt != 0)
                {
                    // Case 1: Existing Debit ≠ 0 → Add incoming Debit
                    debitAmt += transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt,
                        ATBU_Closing_TotalCredit_Amount = 0
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt,
                        ATBU_Closing_TotalDebit_Amount = 0
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
                else if (creditAmt != 0)
                {
                    // Case 2: Existing Credit ≠ 0 → Subtract incoming Debit
                    debitAmt = creditAmt - transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt,
                        ATBU_Closing_TotalCredit_Amount = 0
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
                else
                {
                    // Case 3: Both Debit & Credit = 0 → Just add to Debit
                    debitAmt += transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
            }
            // 🔹 Credit Transaction (transId = 1)
            else if (transId == 1)
            {
                if (creditAmt != 0)
                {
                    // Case 1: Existing Credit ≠ 0 → Add incoming Credit
                    creditAmt += transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt,
                        ATBU_Closing_TotalCredit_Amount = 0.00
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
                else if (debitAmt != 0)
                {
                    // Case 2: Existing Debit ≠ 0 → Subtract incoming Credit
                    creditAmt = debitAmt - transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt,
                        ATBU_Closing_TotalDebit_Amount = 0.00
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
                else
                {
                    // Case 3: Both Debit & Credit = 0 → Just add to Credit
                    creditAmt += transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
            }

            // 🔹 Execute Update if SQL was built
            if (!string.IsNullOrEmpty(updateSql))
            {
                await conn.ExecuteAsync(updateSql, new
                {
                    CustId = custId,
                    CompId = compId,
                    DurtnId = durtnId,
                    BranchId = branchId,
                    DebitAmt = debitAmt,
                    CreditAmt = creditAmt,
                    Deschead = deschead,
                    DescId = descId,
                    DescName = descName
                });
            }
        }



        //SaveGeneralLedger
        public async Task<int[]> SaveGeneralLedgerAsync(int CompId, List<GeneralLedgerDto> dtos)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            int updateOrSave = 0, oper = 0;

            try
            {
                foreach (var dto in dtos)
                {
                    int iPKId = dto.ATBU_ID;

                    // ✅ Step A: Auto-generate ATBU_CODE using Dapper
                    string sql = @"
                SELECT COUNT(*)
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_CustId = @CustId
                  AND ATBU_CompId = @CompId
                  AND ATBU_YearId = @YearId
                  AND ATBU_BranchId = @BranchId
                  AND ATBU_QuarterId = @QuarterId";

                    var count = await connection.ExecuteScalarAsync<int>(sql, new
                    {
                        CustId = dto.ATBU_CustId,
                        CompId = dto.ATBU_CompId,
                        YearId = dto.ATBU_YEARId,
                        BranchId = dto.ATBU_Branchid,
                        QuarterId = dto.ATBU_QuarterId
                    }, transaction);

                    // Generate ATBU_CODE → count + 1
                    string generatedCode = $"ATBU-{count + 1}";

                    using (var cmdMaster = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        cmdMaster.CommandType = CommandType.StoredProcedure;

                        cmdMaster.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        cmdMaster.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        cmdMaster.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        cmdMaster.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        cmdMaster.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        cmdMaster.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? "127.0.0.1");
                        cmdMaster.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        cmdMaster.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var output1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var output2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdMaster.Parameters.Add(output1);
                        cmdMaster.Parameters.Add(output2);

                        await cmdMaster.ExecuteNonQueryAsync();

                        updateOrSave = (int)(output1.Value ?? 0);
                        oper = (int)(output2.Value ?? 0);
                    }

                    // ✅ Step B: Auto-generate ATBUD_CODE using Dapper
                    string sqlCheckDetail = @"
    SELECT COUNT(*)
    FROM Acc_TrailBalance_Upload_Details
    WHERE ATBUD_CustId = @CustId
      AND ATBUD_CompId = @CompId
      AND ATBUD_YearId = @YearId
      AND Atbud_Branchnameid = @BranchId
      AND ATBUD_QuarterId = @QuarterId";

                    var detailCount = await connection.ExecuteScalarAsync<int>(sqlCheckDetail, new
                    {
                        CustId = dto.ATBUD_CustId,
                        CompId = dto.ATBUD_CompId,
                        YearId = dto.ATBUD_YEARId,   // fixed
                        BranchId = dto.ATBUD_Branchid, // fixed
                        QuarterId = dto.ATBUD_QuarterId
                    }, transaction);

                    // Generate ATBUD_CODE → detailCount + 1
                    string generatedDetailCode = $"ATBUD-{detailCount + 1}";

                    // --- Detail Insert ---
                    using (var cmdDetail = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                    {
                        cmdDetail.CommandType = CommandType.StoredProcedure;

                        cmdDetail.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Masid", oper);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_SubItemid", dto.ATBUD_Subitemid);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? "127.0.0.1");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBU_YEARId);

                        var output1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var output2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdDetail.Parameters.Add(output1);
                        cmdDetail.Parameters.Add(output2);

                        await cmdDetail.ExecuteNonQueryAsync();
                    }
                }
                transaction.Commit();
                return new int[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //ActivateJE
        public async Task<int> ActivateJournalEntriesAsync(ActivateRequestDto dto)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();


            const string sqlMaster = @"
UPDATE Acc_JE_Master
SET Acc_JE_Status = @Status,
    Acc_JE_IPAddress = @IpAddress
WHERE Acc_JE_ID IN @Ids
  AND Acc_JE_CompID = @CompId";

            await connection.ExecuteAsync(sqlMaster, new
            {
                Status = dto.Status,
                IpAddress = dto.IpAddress,
                Ids = dto.DescriptionIds,
                CompId = dto.CompId
            });


            const string sqlFetchDetail = @"
SELECT 
    AJTB_ID, Ajtb_Masid, AJTB_CustId, AJTB_Debit, AJTB_Credit,
    AJTB_BranchId, AJTB_QuarterId, AJTB_Deschead,
    AJTB_Desc, AJTB_DescName
FROM Acc_JETransactions_Details
WHERE Ajtb_Masid IN @Ids
  AND AJTB_CompID = @CompId";

            var detailRows = (await connection.QueryAsync<dynamic>(sqlFetchDetail, new
            {
                Ids = dto.DescriptionIds,
                CompId = dto.CompId
            })).ToList();

            if (!detailRows.Any()) return 0;
         
            string updateStatusSql;

            if (dto.Status == "A")
            {
                // This was missing — now status for A updates correctly
                updateStatusSql = @"
UPDATE Acc_JETransactions_Details
SET AJTB_Status = 'A',
    AJTB_IPAddress = @IpAddress
WHERE Ajtb_Masid IN @Ids
  AND AJTB_CompID = @CompId";
            }
            else
            {
                // Deactivation (already correct in your old version)
                updateStatusSql = @"
UPDATE Acc_JETransactions_Details
SET AJTB_Status = 'D',
    AJTB_IPAddress = @IpAddress
WHERE Ajtb_Masid IN @Ids
  AND AJTB_CompID = @CompId";
            }

            await connection.ExecuteAsync(updateStatusSql, new
            {
                IpAddress = dto.IpAddress,
                Ids = dto.DescriptionIds,
                CompId = dto.CompId
            });

            foreach (var t in detailRows)
            {
                int transType = t.AJTB_Debit > 0 ? 0 : 1;
                decimal transAmt = t.AJTB_Debit > 0 ? t.AJTB_Debit : t.AJTB_Credit;

                if (dto.Status == "A")
                {
                    await ActivateJeDetAsync(
                        dto.CompId,
                        t.AJTB_CustId,
                        transType,
                        transAmt,
                        t.AJTB_BranchId,
                        t.AJTB_Debit,
                        t.AJTB_Credit,
                        t.AJTB_QuarterId,
                        t.AJTB_Deschead,
                        t.AJTB_Desc,
                        t.AJTB_DescName
                    );
                }
                else if (dto.Status == "D")
                {
                    await DeactivateJeDetAsync(
                        dto.CompId,
                        t.AJTB_CustId,
                        transType,
                        transAmt,
                        t.AJTB_BranchId,
                        t.AJTB_Debit,
                        t.AJTB_Credit,
                        t.AJTB_QuarterId,
                        t.AJTB_Deschead,
                        t.AJTB_Desc,
                        t.AJTB_DescName
                    );
                }
            }
            return detailRows.Count;
        }

        public async Task ActivateJeDetAsync(
     int compId, int custId, int transId, decimal transAmt,
     int branchId, decimal transDbAmt, decimal transCrAmt,
     int durtnId, int deschead, int descId, string descName)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // 🔹 Fetch TB row
            string sql = @"
SELECT TOP 1 *
FROM Acc_TrailBalance_Upload
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new
            {
                CustId = custId,
                CompId = compId,
                DurtnId = durtnId,
                BranchId = branchId,
                Deschead = deschead
            });

            if (row == null) return;

            decimal debitAmt = (decimal?)row.ATBU_Closing_TotalDebit_Amount ?? 0m;
            decimal creditAmt = (decimal?)row.ATBU_Closing_TotalCredit_Amount ?? 0m;

            string updateSql = string.Empty;

            // -------------------------------
            // 🔹 TRANSACTION TYPE = 0 → DEBIT
            // -------------------------------
            if (transId == 0)
            {
                // Add back Debit (reverse of Deactivate)
                if (debitAmt != 0)
                {
                    debitAmt += transDbAmt;

                    updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";
                }
                else if (creditAmt != 0)
                {
                    debitAmt = creditAmt - transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";

                        debitAmt = Math.Abs(debitAmt);
                    }
                }
                else
                {
                    debitAmt += transDbAmt;

                    updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";
                }
            }
            // -------------------------------
            // 🔹 TRANSACTION TYPE = 1 → CREDIT
            // -------------------------------
            else if (transId == 1)
            {
                if (creditAmt != 0)
                {
                    creditAmt += transCrAmt;

                    updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";
                }
                else if (debitAmt != 0)
                {
                    creditAmt = debitAmt - transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";

                        creditAmt = Math.Abs(creditAmt);
                    }
                }
                else
                {
                    creditAmt += transCrAmt;

                    updateSql = @"
UPDATE Acc_TrailBalance_Upload
SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
WHERE ATBU_CustId = @CustId
  AND ATBU_CompID = @CompId
  AND ATBU_QuarterId = @DurtnId
  AND ATBU_ID = @Deschead
  AND ATBU_BranchId = @BranchId";
                }
            }

            // ------------------------------------
            // 🔹 EXECUTE FINAL SQL
            // ------------------------------------
            if (!string.IsNullOrEmpty(updateSql))
            {
                await conn.ExecuteAsync(updateSql, new
                {
                    CustId = custId,
                    CompId = compId,
                    DurtnId = durtnId,
                    BranchId = branchId,
                    DebitAmt = debitAmt,
                    CreditAmt = creditAmt,
                    Deschead = deschead,
                    DescId = descId,
                    DescName = descName
                });
            }
        }

        public async Task DeactivateJeDetAsync(int compId,int custId,int transId, decimal transAmt,
            int branchId,decimal transDbAmt,decimal transCrAmt,int durtnId,int deschead,int descId, string descName)     
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            var connectionString = _configuration.GetConnectionString(dbName);
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // 🔹 Select the Trail Balance row for given parameters
            string sql = @"
        SELECT TOP 1 *
        FROM Acc_TrailBalance_Upload
        WHERE ATBU_CustId = @CustId
          AND ATBU_CompID = @CompId
          AND ATBU_QuarterId = @DurtnId
          AND ATBU_ID = @Deschead
          AND ATBU_BranchId = @BranchId";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new
            {
                CustId = custId,
                CompId = compId,
                DurtnId = durtnId,
                BranchId = branchId,
                Deschead = deschead
            });

            if (row == null) return;

            // 🔹 Safe cast of values
            decimal debitAmt = (decimal?)row.ATBU_Closing_TotalDebit_Amount ?? 0m;
            decimal creditAmt = (decimal?)row.ATBU_Closing_TotalCredit_Amount ?? 0m;

            string updateSql = string.Empty;

            // 🔹 Debit Transaction (transId = 0)
            if (transId == 0)
            {
                if (debitAmt != 0)
                {
                    // Case 1: Existing Debit ≠ 0 → Add incoming Debit
                    debitAmt -= transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
                                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
                                           WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
                else if (creditAmt != 0)
                {
                    // Case 2: Existing Credit ≠ 0 → Subtract incoming Debit
                    debitAmt = creditAmt + transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
                else
                {
                    // Case 3: Both Debit & Credit = 0 → Just add to Debit
                    debitAmt -= transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
            }
            // 🔹 Credit Transaction (transId = 1)
            else if (transId == 1)
            {
                if (creditAmt != 0)
                {
                    // Case 1: Existing Credit ≠ 0 → Add incoming Credit
                    creditAmt -= transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
                else if (debitAmt != 0)
                {
                    // Case 2: Existing Debit ≠ 0 → Subtract incoming Credit
                    creditAmt = debitAmt + transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
                else
                {
                    // Case 3: Both Debit & Credit = 0 → Just add to Credit
                    creditAmt -= transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
            }

            // 🔹 Execute Update if SQL was built
            if (!string.IsNullOrEmpty(updateSql))
            {
                await conn.ExecuteAsync(updateSql, new
                {
                    CustId = custId,
                    CompId = compId,
                    DurtnId = durtnId,
                    BranchId = branchId,
                    DebitAmt = debitAmt,
                    CreditAmt = creditAmt,
                    Deschead = deschead,
                    DescId = descId,
                    DescName = descName
                });
            }
        }



        public async Task<JERecordDto?> GetJERecordAsync(int jeId, int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM Acc_JE_Master WHERE Acc_JE_ID = @JEId AND Acc_JE_CompID = @CompId";

            var result = await connection.QueryFirstOrDefaultAsync<JERecordDto>(sql, new { JEId = jeId, CompId = compId });
            return result;
        }
        public async Task<List<TransactionDetailDto>> LoadTransactionDetailsAsync(int companyId, int yearId, int custId, int jeId, int branchId, int durationId)
        {
            var transactions = new List<TransactionDetailDto>();

            // Dynamic connection string
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);


            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var sql = @"SELECT Ajtb_id, ajtb_deschead, ATBU_Description, AJTB_Debit, AJTB_Credit,c.Acc_JE_BillType,c.Acc_JE_BillDate,c.Acc_JE_Comnments
                        FROM Acc_JETransactions_Details
                        LEFT JOIN Acc_TrailBalance_Upload b ON b.ATBU_ID = ajtb_deschead
                        LEFT JOIN Acc_JE_Master c ON c.Acc_JE_ID = Ajtb_Masid
                        WHERE ajtb_custid = @CustID AND AJTB_CompID = @CompanyID AND AJTB_YearID = @YearID
                        AND c.Acc_JE_ID = @JEID";

                if (branchId != 0)
                {
                    sql += " AND ajtb_BranchID = @BranchID AND Acc_JE_QuarterId = @DurationID";
                }

                sql += " ORDER BY AJTB_ID";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CustID", custId);
                cmd.Parameters.AddWithValue("@CompanyID", companyId);
                cmd.Parameters.AddWithValue("@YearID", yearId);
                cmd.Parameters.AddWithValue("@JEID", jeId);
                if (branchId != 0)
                {
                    cmd.Parameters.AddWithValue("@BranchID", branchId);
                    cmd.Parameters.AddWithValue("@DurationID", durationId);
                }

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    int srNo = 1;
                    while (await reader.ReadAsync())
                    {
                        transactions.Add(new TransactionDetailDto
                        {
                            SrNo = srNo++,
                            DetID = reader["Ajtb_id"] as int?,
                            HeadID = reader["ajtb_deschead"] as int?,
                            GLID = reader["ajtb_deschead"] as int?,
                            GLCode = "",
                            GLDescription = reader["ATBU_Description"]?.ToString(),
                            SubGL = "",
                            Debit = reader["AJTB_Debit"] != DBNull.Value ? Convert.ToDecimal(reader["AJTB_Debit"]) : 0,
                            Credit = reader["AJTB_Credit"] != DBNull.Value ? Convert.ToDecimal(reader["AJTB_Credit"]) : 0,
                            Balance = 0, // You can calculate balance if needed
                            BillDate = reader["Acc_JE_BillDate"]?.ToString(),
                            BillType = reader["Acc_JE_BillType"] as int?,
                            comments = reader["Acc_JE_Comnments"]?.ToString(),
                        });
                    }
                }
            }

            return transactions;
        }

        public async Task<string> GenerateTransactionNoAsync(int Yearid,int Custid, int duration,int branchid)
        {
            try
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Get the connection string
                var connectionString = _configuration.GetConnectionString(dbName);

                // ✅ Step 3: Use SqlConnection
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string sql = $@"
                SELECT ISNULL(MAX(Acc_JE_ID) + 1, 1) 
                FROM Acc_JE_Master 
                WHERE Acc_JE_YearID = {Yearid} 
                  AND Acc_JE_Party = {Custid} 
                  AND Acc_JE_QuarterId = {duration} 
                  AND acc_JE_BranchId = {branchid}";

                int iMax = await connection.ExecuteScalarAsync<int>(sql);

                string prefix = $"JE00-{iMax}";

                return prefix;
            }
            catch (Exception ex)
            {
                throw; // or handle logging
            }
        }
        public async Task<(int UpdateOrSave, int Oper, int FinalId, string FinalCode)> SaveOrUpdateAsync(AdminMasterDto dto)
        {
            try
            {
                // ✅ Get dbName from session first
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Get connection string
                var connectionString = _configuration.GetConnectionString(dbName);
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception($"No connection string found for database: {dbName}");

                // ✅ If new record → generate new Id & Code
                if (dto.Id == 0)
                {
                    dto.Id = await GetMaxIdAsync(dto.CompId, "Content_Management_Master", "cmm_ID", "Cmm_CompID");
                    dto.Code = "JE_" + dto.Id;
                    // ⚠️ Don't reset dto.Id back to 0, let SP handle the logic
                }

                using var conn = new SqlConnection(connectionString);
                using var cmd = new SqlCommand("spContent_Management_Master", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@cmm_ID", dto.Id);
                cmd.Parameters.AddWithValue("@cmm_Code", dto.Code ?? "");
                cmd.Parameters.AddWithValue("@cmm_Desc", dto.Desc ?? "");
                cmd.Parameters.AddWithValue("@cmm_Category", dto.Category ?? "");
                cmd.Parameters.AddWithValue("@cms_Remarks", dto.Remarks ?? "");
                cmd.Parameters.AddWithValue("@cms_KeyComponent", dto.KeyComponent);
                cmd.Parameters.AddWithValue("@cms_Module", dto.Module ?? "");
                cmd.Parameters.AddWithValue("@CMM_RiskCategory", dto.RiskCategory);
                cmd.Parameters.AddWithValue("@CMM_Status", dto.Status ?? "");
                cmd.Parameters.AddWithValue("@cmm_Rate", dto.Rate);
                cmd.Parameters.AddWithValue("@CMM_Act", dto.CMMAct ?? "");
                cmd.Parameters.AddWithValue("@CMM_HSNSAC", dto.CMMHSNSAC ?? "");
                cmd.Parameters.AddWithValue("@cmm_delflag", dto.Delflag ?? "");
                cmd.Parameters.AddWithValue("@CMM_CrBy", dto.CreatedBy);
                cmd.Parameters.AddWithValue("@CMM_UpdatedBy", dto.UpdatedBy);
                cmd.Parameters.AddWithValue("@CMM_IpAddress", dto.IpAddress ?? "");
                cmd.Parameters.AddWithValue("@CMM_CompId", dto.CompId);

                var outUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outOper = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(outUpdateOrSave);
                cmd.Parameters.Add(outOper);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (
                    Convert.ToInt32(outUpdateOrSave.Value),
                    Convert.ToInt32(outOper.Value),
                    dto.Id,
                    dto.Code
                );
            }
            catch (SqlException sqlEx)
            {
                // Database related errors
                throw new Exception($"SQL Error in SaveOrUpdateAsync: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Any other unhandled error
                throw new Exception($"Unexpected error in SaveOrUpdateAsync: {ex.Message}", ex);
            }
        }


        private async Task<int> GetMaxIdAsync(int compId, string table, string column, string compColumn)
        {
            var sql = $"SELECT ISNULL(MAX({column}) + 1, 1) FROM {table} WHERE {compColumn} = @CompId";

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);
            using var conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CompId", compId);
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        //GetJETypeDropDown
        public async Task<IEnumerable<JeTypeDto>> GetJETypeListAsync(int CompId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: SQL Query
            var query = @"
  SELECT 
      CMM_ID AS JeTypeId,
      CMM_Desc AS JeTypeName
  FROM Content_Management_Master
  WHERE CMM_CompId = @CompId
    AND CMM_Category = 'JE'
    AND CMM_DELFLAG = 'A'";

            // ✅ Step 5: Execute and return
            return await connection.QueryAsync<JeTypeDto>(query, new { CompID = CompId });
        }

        //GetJETypeDropDownDetails
        public async Task<IEnumerable<JETypeDropDownDetailsDto>> GetJETypeDropDownDetailsAsync(int compId, int custId, int yearId, int BranchId, int jetype)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get Connection String
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 3: SQL base query
            var sql = @"
    SELECT 
        je.Acc_JE_ID, 
        je.Acc_JE_TransactionNo, 
        je.acc_JE_BranchId, 
        je.Acc_JE_BillDate AS BillDate,
        cmm.cmm_Desc AS BillType, 
        je.Acc_JE_Party, 
        je.Acc_JE_Status, 
        je.Acc_JE_Comnments, 
        je.acc_JE_QuarterId,
     scm.CUST_NAME,
     ajtb.AJTB_Credit,
     ajtb.AJTB_Debit,
     ajtb.AJTB_DescName
    FROM Acc_JE_Master je
    LEFT JOIN Content_Management_Master cmm 
        ON cmm.cmm_id = je.Acc_JE_BillType
        AND cmm.cmm_category = 'JE'
    LEFT JOIN SAD_Customer_master scm
        ON scm.CUST_ID = je.Acc_JE_Party
    LEFT JOIN Acc_JETransactions_Details ajtb
        ON ajtb.Ajtb_Masid = je.Acc_JE_ID
    WHERE je.Acc_JE_Party = @custId 
      AND je.Acc_JE_CompID = @compId 
      AND je.Acc_JE_YearId = @yearId
      AND je.Acc_JE_BranchId = @BranchId 
";

            // Step 4: Add conditional filter
            if (jetype > 0)
            {
                sql += " AND je.Acc_JE_BillType = @jetype ";
            }
            sql += " ORDER BY je.Acc_JE_ID ASC";

            // Step 5: Execute
            var result = await connection.QueryAsync<JETypeDropDownDetailsDto>(
                sql,
                new { compId, custId, yearId, BranchId, jetype }
            );
            return result.ToList();
        }

        //SaveJEType
        public async Task<string> SaveOrUpdateContentForJEAsync(
      int? id,
      int compId,
      string description,
      string remarks,
      string Category)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ STEP 1: Duplicate check
            var duplicateCount = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*) 
          FROM Content_Management_Master
          WHERE cmm_Desc = @Desc
            AND cmm_Category = @Category
            AND Cmm_CompID = @CompID
            AND (@Id IS NULL OR cmm_ID <> @Id)",
                new
                {
                    Desc = description.Trim(),
                    Category = Category.Trim(),
                    CompID = compId,
                    Id = id
                });

            if (duplicateCount > 0)
                return "ALREADY_EXISTS";

            string newCode;

            // ✅ STEP 2: Update
            if (id.HasValue && id > 0)
            {
                await connection.ExecuteAsync(
                    @"UPDATE Content_Management_Master
              SET cmm_Desc = @Desc,
                  cms_Remarks = @Remarks,
                  cmm_Delflag = 'A',
                  CMM_Status = 'A',
                  cmm_Category = @Category
              WHERE cmm_ID = @Id AND Cmm_CompID = @CompID",
                    new
                    {
                        Id = id,
                        Desc = description,
                        Remarks = remarks,
                        CompID = compId,
                        Category
                    });

                newCode = $"JE_{id}";
            }
            // ✅ STEP 3: Insert
            else
            {
                int newId = await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(cmm_ID), 0) + 1 
              FROM Content_Management_Master 
              WHERE Cmm_CompID = @CompID",
                    new { CompID = compId });

                newCode = $"JE_{newId}";

                await connection.ExecuteAsync(
                    @"INSERT INTO Content_Management_Master
              (cmm_ID, cmm_Code, cmm_Desc, Cmm_CompID, cms_Remarks, cmm_Delflag, CMM_Status, cmm_Category)
              VALUES
              (@Id, @Code, @Desc, @CompID, @Remarks, 'A', 'A', @Category)",
                    new
                    {
                        Id = newId,
                        Code = newCode,
                        Desc = description,
                        CompID = compId,
                        Remarks = remarks,
                        Category
                    });
            }

            return newCode;
        }
        public async Task<JournalEntryProcessResponse> ProcessJournalEntriesAsync(JournalEntryUploadDto uploadDto)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = new JournalEntryProcessResponse();

            try
            {

                // Read Excel file
                var rows = await ReadExcelFileAsync(uploadDto.File.OpenReadStream());
                response.TotalRecords = rows.Count;

                if (rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No valid records found in the Excel file";
                    return response;
                }

                // Process in optimized batches
                var batchResult = await ProcessLargeDatasetOptimizedAsync(rows, uploadDto);

                response.Success = batchResult.Success;
                response.ProcessedRecords = batchResult.ProcessedCount;
                response.FailedRecords = batchResult.FailedCount;
                response.Errors = batchResult.Errors;
                response.ProcessingTime = stopwatch.Elapsed;

                response.Message = batchResult.Success
                    ? $"Successfully processed {batchResult.ProcessedCount} records in {stopwatch.Elapsed.TotalSeconds:F2} seconds"
                    : $"Processed {batchResult.ProcessedCount} records, {batchResult.FailedCount} failed";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error processing file: " + ex.Message;
            }

            return response;
        }

        private async Task<BatchProcessResult> ProcessLargeDatasetOptimizedAsync(
            List<JournalEntryRowDto> rows,
            JournalEntryUploadDto uploadDto)
        {
            var result = new BatchProcessResult();
            var batchSize = 5000; // Increased batch size for better performance

            // Get connection string
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. PRE-COMPUTE ALL REQUIRED DATA (Single round-trip)
                var preComputedData = await PreComputeAllDataAsync(connection, transaction, rows, uploadDto);

                if (!preComputedData.Success)
                {
                    transaction.Rollback();
                    result.Errors.AddRange(preComputedData.Errors);
                    return result;
                }

                // 2. BULK CREATE MISSING ACCOUNTS (Single operation)
                await BulkCreateAccountsAsync(connection, transaction, preComputedData.AccountsToCreate, uploadDto);

                // 3. PROCESS IN BATCHES WITH BULK OPERATIONS
                var batches = rows.Chunk(batchSize);
                int processedCount = 0;

                foreach (var batch in batches)
                {
                    var batchResult = await ProcessBatchWithBulkOperationsAsync(
                        connection, transaction, batch.ToList(), preComputedData, uploadDto);

                    processedCount += batchResult.ProcessedCount;
                    result.FailedCount += batchResult.FailedCount;
                    result.Errors.AddRange(batchResult.Errors);

                    if (!batchResult.Success && batchResult.Errors.Any(e => e.Contains("Fatal")))
                    {
                        transaction.Rollback();
                        result.Success = false;
                        return result;
                    }
                }

                // 4. UPDATE TRAIL BALANCE IN BULK
                await UpdateTrailBalanceBulkAsync(connection, transaction, preComputedData, uploadDto);

                transaction.Commit();
                result.Success = true;
                result.ProcessedCount = processedCount;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.Errors.Add($"Transaction failed: {ex.Message}");
                result.Success = false;
            }

            return result;
        }

        private async Task<PreComputedData> PreComputeAllDataAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            List<JournalEntryRowDto> rows,
            JournalEntryUploadDto uploadDto)
        {
            var result = new PreComputedData();

            // 1. Get distinct voucher types and accounts in one go
            var distinctVoucherTypes = rows
                .Where(r => !string.IsNullOrEmpty(r.VchType))
                .Select(r => r.VchType)
                .Distinct()
                .ToList();

            var distinctAccounts = rows
                .Where(r => !string.IsNullOrEmpty(r.Particulars))
                .Select(r => r.Particulars)
                .Distinct()
                .ToList();

            // 2. Batch fetch voucher type IDs
            var voucherTypeIds = await BatchGetVoucherTypeIdsAsync(
                connection, transaction, distinctVoucherTypes, uploadDto.AccessCodeId);

            result.VoucherTypeCache = voucherTypeIds;

            // 3. Batch check existing accounts
            var existingAccounts = await BatchCheckAccountsAsync(
                connection, transaction, distinctAccounts, uploadDto);

            // 4. Identify accounts to create
            foreach (var account in distinctAccounts)
            {
                if (!existingAccounts.ContainsKey(account) || existingAccounts[account] == 0)
                {
                    result.AccountsToCreate.Add(account);
                }
                else
                {
                    result.AccountIdCache[account] = existingAccounts[account];
                }
            }

            // 5. Validate dates
            int lineNo = 1;
            foreach (var row in rows)
            {
                if (!string.IsNullOrEmpty(row.Date) && !IsValidDate(row.Date))
                {
                    result.Errors.Add($"Invalid date format at line {lineNo}: {row.Date}");
                }
                lineNo++;
            }

            result.Success = !result.Errors.Any();
            return result;
        }

        private async Task BulkCreateAccountsAsync(
         SqlConnection connection,
         SqlTransaction transaction,
         List<string> accountsToCreate,
         JournalEntryUploadDto uploadDto)
        {
            if (!accountsToCreate.Any()) return;

            // Get the next available ATBU_ID
            var nextId = await GetNextATBU_IDAsync(connection, transaction);

            // Create DataTable for bulk insert
            var dt = new DataTable();
            dt.Columns.Add("ATBU_ID", typeof(int)); // Add this required column
            dt.Columns.Add("ATBU_CODE", typeof(string));
            dt.Columns.Add("ATBU_Description", typeof(string));
            dt.Columns.Add("ATBU_CustId", typeof(int));
            dt.Columns.Add("ATBU_CompId", typeof(int));
            dt.Columns.Add("ATBU_YEARId", typeof(int));
            dt.Columns.Add("ATBU_Branchid", typeof(int));
            dt.Columns.Add("ATBU_QuarterId", typeof(int));
            dt.Columns.Add("ATBU_CRBY", typeof(int));
            // Add other required columns that cannot be NULL...

            // Get starting code
            var maxCount = await GetMaxAccountCodeAsync(connection, transaction, uploadDto);

            foreach (var account in accountsToCreate)
            {
                maxCount++;
                dt.Rows.Add(
                    nextId++, // Provide value for ATBU_ID
                    $"SCh00{maxCount}",
                    account,
                    uploadDto.CustomerId,
                    uploadDto.AccessCodeId,
                    uploadDto.FinancialYearId,
                    uploadDto.BranchId,
                    uploadDto.DurationId,
                    uploadDto.UserId
                );
            }

            // Bulk insert using SqlBulkCopy
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = "Acc_TrailBalance_Upload";
            bulkCopy.BatchSize = 1000;
            bulkCopy.BulkCopyTimeout = 300; // 5 minutes

            // Map columns
            foreach (DataColumn column in dt.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dt);
        }

        private async Task<int> GetNextATBU_IDAsync(SqlConnection connection, SqlTransaction transaction)
        {
            // Query to get the maximum ATBU_ID currently in the table
            using var cmd = new SqlCommand(
                "SELECT ISNULL(MAX(ATBU_ID), 0) + 1 FROM Acc_TrailBalance_Upload",
                connection,
                transaction);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private async Task<BatchProcessResult> ProcessBatchWithBulkOperationsAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            List<JournalEntryRowDto> batch,
            PreComputedData preComputedData,
            JournalEntryUploadDto uploadDto)
        {
            var result = new BatchProcessResult();
            var journalEntries = new List<JournalEntryMaster>();
            var transactionDetails = new List<JournalEntryDetail>();
            var trailBalanceUpdates = new List<TrailBalanceUpdate>();

            try
            {
                // Generate transaction numbers in bulk
                var transactionNumbers = await GenerateTransactionNumbersAsync(
                    connection, transaction, batch.Count, uploadDto);

                int transactionIndex = 0;

                foreach (var row in batch)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(row.Particulars)) continue;

                        // Get pre-computed values
                        var voucherTypeId = preComputedData.VoucherTypeCache.TryGetValue(row.VchType ?? "", out var vtId)
                            ? vtId : 0;

                        var accountId = preComputedData.AccountIdCache.TryGetValue(row.Particulars, out var accId)
                            ? accId : 0;

                        if (accountId == 0)
                        {
                            result.FailedCount++;
                            result.Errors.Add($"Account not found: {row.Particulars}");
                            continue;
                        }

                        var transactionNo = transactionNumbers[transactionIndex];
                        var transactionDate = ParseTransactionDate(row.Date);

                        // Create journal entry master
                        var journalEntry = new JournalEntryMaster
                        {
                            AJTB_TranscNo = transactionNo,
                            Acc_JE_Party = uploadDto.CustomerId,
                            Acc_JE_BillType = voucherTypeId,
                            Acc_JE_BillNo = row.VchNo ?? "",
                            Acc_JE_BillDate = transactionDate,
                            Acc_JE_BillAmount = row.Debit + row.Credit,
                            Acc_JE_YearID = uploadDto.FinancialYearId,
                            Acc_JE_CompID = uploadDto.AccessCodeId,
                            Acc_JE_CreatedBy = uploadDto.UserId,
                            acc_JE_BranchId = uploadDto.BranchId,
                            Acc_JE_Quarterly = uploadDto.DurationId
                        };
                        journalEntries.Add(journalEntry);

                        // Create transaction details
                        if (row.Debit > 0)
                        {
                            transactionDetails.Add(new JournalEntryDetail
                            {
                                AJTB_TranscNo = transactionNo,
                                AJTB_CustId = uploadDto.CustomerId,
                                AJTB_Desc = accountId,
                                AJTB_DescName = row.Particulars,
                                AJTB_Debit = row.Debit,
                                AJTB_Credit = 0,
                                AJTB_YearID = uploadDto.FinancialYearId,
                                AJTB_CompID = uploadDto.AccessCodeId,
                                AJTB_BranchId = uploadDto.BranchId,
                                AJTB_QuarterId = uploadDto.DurationId,
                                AJTB_CreatedOn = transactionDate
                            });

                            trailBalanceUpdates.Add(new TrailBalanceUpdate
                            {
                                AccountId = accountId,
                                CustomerId = uploadDto.CustomerId,
                                DebitAmount = row.Debit,
                                CreditAmount = 0,
                                BranchId = uploadDto.BranchId,
                                DurationId = uploadDto.DurationId,
                                CompId = uploadDto.AccessCodeId
                            });
                        }

                        if (row.Credit > 0)
                        {
                            transactionDetails.Add(new JournalEntryDetail
                            {
                                AJTB_TranscNo = transactionNo,
                                AJTB_CustId = uploadDto.CustomerId,
                                AJTB_Desc = accountId,
                                AJTB_DescName = row.Particulars,
                                AJTB_Debit = 0,
                                AJTB_Credit = row.Credit,
                                AJTB_YearID = uploadDto.FinancialYearId,
                                AJTB_CompID = uploadDto.AccessCodeId,
                                AJTB_BranchId = uploadDto.BranchId,
                                AJTB_QuarterId = uploadDto.DurationId,
                                AJTB_CreatedOn = transactionDate
                            });

                            trailBalanceUpdates.Add(new TrailBalanceUpdate
                            {
                                AccountId = accountId,
                                CustomerId = uploadDto.CustomerId,
                                DebitAmount = 0,
                                CreditAmount = row.Credit,
                                BranchId = uploadDto.BranchId,
                                DurationId = uploadDto.DurationId,
                                CompId = uploadDto.AccessCodeId
                            });
                        }

                        transactionIndex++;
                        result.ProcessedCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailedCount++;
                        result.Errors.Add($"Row error: {ex.Message}");
                    }
                }

                // BULK INSERT Journal Entries
                if (journalEntries.Any())
                {
                    await _bulkService.BulkInsertJournalEntriesAsync(connection, transaction, journalEntries);
                }

                // BULK INSERT Transaction Details
                if (transactionDetails.Any())
                {
                    await _bulkService.BulkInsertTransactionDetailsAsync(connection, transaction, transactionDetails);
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Batch processing failed: {ex.Message}");
                result.Success = false;
            }

            return result;
        }
      
        private async Task UpdateTrailBalanceBulkAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            PreComputedData preComputedData,
            JournalEntryUploadDto uploadDto)
        {
            // Use MERGE statement or stored procedure for bulk update
            var sql = @"
                UPDATE ATBU 
                SET ATBU_Closing_TotalDebit_Amount = ATBU_Closing_TotalDebit_Amount + @Debit,
                    ATBU_Closing_TotalCredit_Amount = ATBU_Closing_TotalCredit_Amount + @Credit
                FROM Acc_TrailBalance_Upload ATBU
                WHERE ATBU.ATBU_ID = @AccountId
                  AND ATBU.ATBU_CustId = @CustomerId
                  AND ATBU.ATBU_CompId = @CompId
                  AND ATBU.ATBU_Branchid = @BranchId
                  AND ATBU.ATBU_QuarterId = @QuarterId";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.Add("@AccountId", SqlDbType.Int);
            command.Parameters.Add("@CustomerId", SqlDbType.Int);
            command.Parameters.Add("@CompId", SqlDbType.Int);
            command.Parameters.Add("@BranchId", SqlDbType.Int);
            command.Parameters.Add("@QuarterId", SqlDbType.Int);
            command.Parameters.Add("@Debit", SqlDbType.Decimal);
            command.Parameters.Add("@Credit", SqlDbType.Decimal);

            // Batch execute updates
            foreach (var update in preComputedData.TrailBalanceUpdates)
            {
                command.Parameters["@AccountId"].Value = update.AccountId;
                command.Parameters["@CustomerId"].Value = update.CustomerId;
                command.Parameters["@CompId"].Value = update.CompId;
                command.Parameters["@BranchId"].Value = update.BranchId;
                command.Parameters["@QuarterId"].Value = update.DurationId;
                command.Parameters["@Debit"].Value = update.DebitAmount;
                command.Parameters["@Credit"].Value = update.CreditAmount;

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<JournalEntryRowDto>> ReadExcelFileAsync(Stream fileStream)
        {
            var rows = new List<JournalEntryRowDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];

            int rowCount = worksheet.Dimension.Rows;
            string lastParticulars = "";
            string lastVchType = "";
            string lastVchNo = "";

            for (int row = 2; row <= rowCount; row++) // Start from row 2 (skip header)
            {
                try
                {
                    var dateCell = worksheet.Cells[row, 1].Text;
                    var particulars = worksheet.Cells[row, 2].Text?.Trim();
                    var vchType = worksheet.Cells[row, 3].Text?.Trim();
                    var vchNo = worksheet.Cells[row, 4].Text?.Trim();
                    var debitStr = worksheet.Cells[row, 5].Text;
                    var creditStr = worksheet.Cells[row, 6].Text;

                    // Skip if both debit and credit are empty/zero
                    decimal debit = decimal.TryParse(debitStr, out var d) ? d : 0;
                    decimal credit = decimal.TryParse(creditStr, out var c) ? c : 0;

                    if (debit == 0 && credit == 0) continue;
                    if (string.IsNullOrEmpty(particulars)) continue;

                    // Apply carry-forward logic
                    if (!string.IsNullOrEmpty(particulars)) lastParticulars = particulars;
                    if (!string.IsNullOrEmpty(vchType)) lastVchType = vchType;
                    if (!string.IsNullOrEmpty(vchNo)) lastVchNo = vchNo;

                    var rowDto = new JournalEntryRowDto
                    {
                        Date = FormatDate(dateCell),
                        Particulars = string.IsNullOrEmpty(particulars) ? lastParticulars : particulars,
                        VchType = string.IsNullOrEmpty(vchType) ? lastVchType : vchType,
                        VchNo = string.IsNullOrEmpty(vchNo) ? lastVchNo : vchNo,
                        Debit = debit,
                        Credit = credit
                    };

                    rows.Add(rowDto);
                }
                catch (Exception ex)
                {
                }
            }

            return rows;
        }

        private string FormatDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return "";

            try
            {
                if (DateTime.TryParse(dateString, out var date))
                {
                    return date.ToString("dd-MM-yyyy");
                }
            }
            catch
            {
                // Keep original if can't parse
            }

            return dateString;
        }

        private bool IsValidDate(string dateString)
        {
            string[] dateFormats = {
                "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd",
                "dd-MM-yyyy", "MM-dd-yyyy", "dd.MM.yyyy",
                "MMMM dd, yyyy", "dd MMMM yyyy", "yyyy/MM/dd",
                "MMM dd, yyyy", "dd-MMM-yyyy", "yyyyMMdd"
            };

            return DateTime.TryParseExact(dateString, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private DateTime ParseTransactionDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return new DateTime(1900, 1, 1);

            string[] dateFormats = {
                "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd",
                "dd-MM-yyyy", "MM-dd-yyyy", "dd.MM.yyyy",
                "MMMM dd, yyyy", "dd MMMM yyyy", "yyyy/MM/dd",
                "MMM dd, yyyy", "dd-MMM-yyyy", "yyyyMMdd"
            };

            if (DateTime.TryParseExact(dateString, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return DateTime.MinValue;
        }

        // Helper classes for pre-computation
        private class PreComputedData
        {
            public bool Success { get; set; }
            public Dictionary<string, int> VoucherTypeCache { get; set; } = new();
            public Dictionary<string, int> AccountIdCache { get; set; } = new();
            public List<string> AccountsToCreate { get; set; } = new();
            public List<TrailBalanceUpdate> TrailBalanceUpdates { get; set; } = new();
            public List<string> Errors { get; set; } = new();
        }

        private class TrailBalanceUpdate
        {
            public int AccountId { get; set; }
            public int CustomerId { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
            public int BranchId { get; set; }
            public int DurationId { get; set; }
            public int CompId { get; set; }
        }

        // Database helper methods (optimized batch versions)
        private async Task<Dictionary<string, int>> BatchGetVoucherTypeIdsAsync(
            SqlConnection connection, SqlTransaction transaction, List<string> voucherTypes, int compId)
        {
            var result = new Dictionary<string, int>();

            if (!voucherTypes.Any()) return result;

            var sql = @"
                SELECT cmm_Desc, cmm_ID 
                FROM Content_Management_Master 
                WHERE CMM_CompID = @CompID 
                  AND cmm_Category = 'JE' 
                  AND cmm_Delflag = 'A' 
                  AND cmm_Desc IN ({0})";

            var parameters = voucherTypes.Select((_, i) => $"@p{i}").ToArray();
            var inClause = string.Join(",", parameters);
            sql = string.Format(sql, inClause);

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@CompID", compId);

            for (int i = 0; i < voucherTypes.Count; i++)
            {
                command.Parameters.AddWithValue($"@p{i}", voucherTypes[i]);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result[reader.GetString(0)] = reader.GetInt32(1);
            }

            // Set 0 for not found types
            foreach (var type in voucherTypes.Where(t => !result.ContainsKey(t)))
            {
                result[type] = 0;
            }

            return result;
        }

        private async Task<Dictionary<string, int>> BatchCheckAccountsAsync(
            SqlConnection connection, SqlTransaction transaction, List<string> accounts, JournalEntryUploadDto uploadDto)
        {
            var result = new Dictionary<string, int>();

            if (!accounts.Any()) return result;

            var sql = @"
                SELECT ATBU_Description, ATBU_ID 
                FROM Acc_TrailBalance_Upload 
                WHERE ATBU_Description IN ({0})
                  AND ATBU_CustId = @CustId
                  AND ATBU_Branchid = @BranchId
                  AND ATBU_CompId = @CompId
                  AND ATBU_YEARId = @YearId";

            if (uploadDto.DurationId != 0)
            {
                sql += " AND ATBU_QuarterId = @QuarterId";
            }

            var parameters = accounts.Select((_, i) => $"@p{i}").ToArray();
            var inClause = string.Join(",", parameters);
            sql = string.Format(sql, inClause);

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@CustId", uploadDto.CustomerId);
            command.Parameters.AddWithValue("@BranchId", uploadDto.BranchId);
            command.Parameters.AddWithValue("@CompId", uploadDto.AccessCodeId);
            command.Parameters.AddWithValue("@YearId", uploadDto.FinancialYearId);

            if (uploadDto.DurationId != 0)
            {
                command.Parameters.AddWithValue("@QuarterId", uploadDto.DurationId);
            }

            for (int i = 0; i < accounts.Count; i++)
            {
                command.Parameters.AddWithValue($"@p{i}", accounts[i]);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result[reader.GetString(0)] = reader.GetInt32(1);
            }

            return result;
        }

        private async Task<List<string>> GenerateTransactionNumbersAsync(
            SqlConnection connection, SqlTransaction transaction, int count, JournalEntryUploadDto uploadDto)
        {
            var result = new List<string>();
            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(AJTB_TranscNo, 3, LEN(AJTB_TranscNo)) AS INT)), 0) + 1
                FROM Acc_JETransactions_Details
                WHERE AJTB_YearID = @YearId
                  AND AJTB_CustId = @Party
                  AND AJTB_QuarterId = @QuarterId
                  AND AJTB_BranchId = @BranchId";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@YearId", uploadDto.FinancialYearId);
            command.Parameters.AddWithValue("@Party", uploadDto.CustomerId);
            command.Parameters.AddWithValue("@QuarterId", uploadDto.DurationId);
            command.Parameters.AddWithValue("@BranchId", uploadDto.BranchId);

            var startNumber = Convert.ToInt32(await command.ExecuteScalarAsync());

            for (int i = 0; i < count; i++)
            {
                result.Add($"TR{(startNumber + i):D3}");
            }

            return result;
        }

        private async Task<int> GetMaxAccountCodeAsync(SqlConnection connection, SqlTransaction transaction, JournalEntryUploadDto uploadDto)
        {
            var sql = "SELECT COUNT(*) FROM Acc_TrailBalance_Upload WHERE ATBU_CustId = @CustId AND ATBU_CompId = @CompId AND ATBU_YEARId = @YearId AND ATBU_BranchId = @BranchId AND ATBU_QuarterId = @QuarterId";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@CustId", uploadDto.CustomerId);
            command.Parameters.AddWithValue("@CompId", uploadDto.AccessCodeId);
            command.Parameters.AddWithValue("@YearId", uploadDto.FinancialYearId);
            command.Parameters.AddWithValue("@BranchId", uploadDto.BranchId);
            command.Parameters.AddWithValue("@QuarterId", uploadDto.DurationId);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        // Implement other interface methods...
        public Task<JournalEntryProcessResponse> ValidateAndProcessExcelFileAsync(Stream fileStream, JournalEntryUploadDto uploadDto)
        {
            throw new NotImplementedException();
        }

        public Task<BatchProcessResult> ProcessBatchAsync(List<JournalEntryRowDto> batch, int customerId, int financialYearId, int branchId, int durationId, int accessCodeId, int userId, string ipAddress)
        {
            throw new NotImplementedException();
        }
    
}
}




