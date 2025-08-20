using System.Data;
using System.Data.Common;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;
using static TracePca.Service.FIN_statement.JournalEntryService;

namespace TracePca.Service.FIN_statement
{
    public class JournalEntryService : JournalEntryInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public JournalEntryService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        //GetJournalEntryInformation
        public async Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(
             int CompId, int UserId, string Status, int CustId, int YearId, int BranchId, string DateFormat, int DurationId)
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

            var statusFilter = Status switch
            {
                "0" => "A",
                "1" => "D",
                "2" => "W",
                _ => null
            };

            var sql = new StringBuilder();
            sql.Append(@"
        SELECT 
            Acc_JE_ID AS Id,
            Acc_JE_TransactionNo AS TransactionNo,
            acc_JE_BranchId AS BranchID,
            '' AS BillNo,
            FORMAT(Acc_JE_BillDate, @dateFormat) AS BillDate,
            Acc_JE_BillType,
            Acc_JE_Party AS PartyID,
            Acc_JE_Status AS Status
        FROM Acc_JE_Master
        WHERE Acc_JE_Party = @custId 
            AND Acc_JE_CompID = @compId 
            AND Acc_JE_YearId = @yearId");

            if (!string.IsNullOrEmpty(statusFilter))
                sql.Append(" AND Acc_JE_Status = @statusFilter");

            if (BranchId != 0)
                sql.Append(" AND acc_je_BranchID = @branchId");

            if (DurationId != 0)
                sql.Append(" AND Acc_JE_QuarterId = @durationId");

            sql.Append(" ORDER BY Acc_JE_ID ASC");

            var entries = (await connection.QueryAsync<JournalEntryInformationDto>(
                sql.ToString(),
                new { CompId, CustId, YearId, BranchId, DurationId, statusFilter, DateFormat }
            )).ToList();

            foreach (var entry in entries)
            {
                // Get Debit/Credit details
                var detailQuery = @"
            SELECT 
                SUM(AJTB_Debit) AS Debit,
                SUM(AJTB_Credit) AS Credit,
                AJTB_DescName,
                AJTB_Debit AS LineDebit
            FROM Acc_JETransactions_Details 
            WHERE Ajtb_Masid = @entryId AND AJTB_CustId = @custId
            GROUP BY AJTB_DescName, AJTB_Debit";

                var details = await connection.QueryAsync(detailQuery, new { entryId = entry.Id, CustId });

                var debDescriptions = new List<string>();
                var credDescriptions = new List<string>();
                decimal totalDebit = 0, totalCredit = 0;

                foreach (var row in details)
                {
                    if ((decimal)row.LineDebit != 0)
                    {
                        totalDebit += row.Debit ?? 0;
                        debDescriptions.Add(row.AJTB_DescName);
                    }
                    else
                    {
                        totalCredit += row.Credit ?? 0;
                        credDescriptions.Add(row.AJTB_DescName);
                    }
                }

                entry.Debit = totalDebit;
                entry.Credit = totalCredit;
                entry.DebDescription = string.Join(", ", debDescriptions);
                entry.CredDescription = string.Join(", ", credDescriptions);

                // Map Bill Type
                entry.BillType = entry.BillType switch
                {
                    "1" => "Payment",
                    "2" => "Receipt",
                    "3" => "Petty Cash",
                    "4" => "Purchase",
                    "5" => "Sales",
                    "6" => "Others",
                    _ => ""
                };

                // Map Status
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
          AND Acc_JE_Status <> 'D'
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
            ATBU_STATUS = 'C' AND 
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

            int updateOrSave = 0, oper = 0;

            try
            {
                foreach (var dto in dtos)
                {
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
                                cmdDetail.Parameters.AddWithValue("@AJTB_TranscNo", t.AJTB_TranscNo ?? string.Empty);
                                cmdDetail.Parameters.AddWithValue("@AJTB_CustId", t.AJTB_CustId);
                                cmdDetail.Parameters.AddWithValue("@AJTB_ScheduleTypeid", t.AJTB_ScheduleTypeid);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Deschead", t.AJTB_Deschead);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Desc", t.AJTB_Desc);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Debit", t.AJTB_Debit);
                                cmdDetail.Parameters.AddWithValue("@AJTB_Credit", t.AJTB_Credit);
                                cmdDetail.Parameters.AddWithValue("@AJTB_CreatedBy", t.AJTB_CreatedBy);
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
      AND ATBUD_BranchId = @BranchId
      AND ATBUD_QuarterId = @QuarterId";

                    var detailCount = await connection.ExecuteScalarAsync<int>(sqlCheckDetail, new
                    {
                        CustId = dto.ATBUD_CustId,
                        CompId = dto.ATBUD_CompId,
                        YearId = dto.ATBU_YEARId,
                        BranchId = dto.ATBUD_Branchid,
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
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"
        UPDATE Acc_JE_Master
        SET Acc_JE_Status = @Status,
            Acc_JE_IPAddress = @IpAddress
        WHERE Acc_JE_ID IN @Ids
          AND Acc_JE_CompID = @CompId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Status = "A",          // Activate
                IpAddress = dto.IpAddress,
                Ids = dto.DescriptionIds,
                CompId = dto.CompId
            });

            return rowsAffected;
        }

        //DeActiveteJE
        public async Task<int> ApproveJournalEntriesAsync(ApproveRequestDto dto)
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

            // In VB code, status was "W" (waiting for approval).
            const string sql = @"
        UPDATE Acc_JE_Master
        SET Acc_JE_Status = @Status,
            Acc_JE_IPAddress = @IpAddress
        WHERE Acc_JE_ID IN @Ids
          AND Acc_JE_CompID = @CompId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Status = "W", // waiting for approval
                IpAddress = dto.IpAddress,
                Ids = dto.DescriptionIds,
                CompId = dto.CompId
            });

            return rowsAffected;
        }

    }
}

