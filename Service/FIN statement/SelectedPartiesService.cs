using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

namespace TracePca.Service.FIN_statement
{
    public class SelectedPartiesService : SelectedPartiesInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SelectedPartiesService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetSelectedParties
        public async Task<IEnumerable<LoadTrailBalanceDto>> GetTrailBalanceAsync(int custId, int financialYearId, int branchId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var query = @"
        SELECT ATBU_Description,ATBU_ID,
            ATBU_Closing_TotalDebit_Amount, 
            ATBU_Closing_TotalCredit_Amount, ATBU_Delflg    
            FROM Acc_TrailBalance_Upload
        WHERE ATBU_CustId = @CustId
          AND ATBU_YEARId = @FinancialYearId
          AND ATBU_Branchid = @BranchId
        ORDER BY ATBU_Id DESC";
            return await connection.QueryAsync<LoadTrailBalanceDto>(query, new
            {
                CustId = custId,
                FinancialYearId = financialYearId,
                BranchId = branchId
            });
        }

        //UpdateSelectedPartiesStatus
        public async Task<int> UpdateTrailBalanceStatusAsync(List<UpdateTrailBalanceStatusDto> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
                return 0; // Nothing to update

            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Build parameterized CASE statement
                var caseStatements = dtoList
                    .Select((d, i) => $"WHEN @Id{i} THEN @Status{i}")
                    .ToList();

                var sql = $@"
          UPDATE Acc_TrailBalance_Upload
          SET ATBU_DELFLG = CASE ATBU_ID
              {string.Join(" ", caseStatements)}
          END
          WHERE ATBU_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"Status{i}", dtoList[i].Status);
                }

                var updatedCount = await connection.ExecuteAsync(sql, parameters, transaction);

                transaction.Commit();
                return updatedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //GetJETransactionDetails
        public async Task<IEnumerable<JournalEntryWithTrailBalanceDto>> GetJournalEntryWithTrailBalanceAsync(int custId, int yearId, int branchId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: SQL Query
            var query = @"
        SELECT 
            JED.AJTB_ID,
            JED.AJTB_DescName,
          JE.ACC_JE_BILLNO as AJTB_TranscNo,
            JED.AJTB_CreatedOn,
            JED.AJTB_Debit,
            JED.AJTB_Credit,
            TBU.ATBU_Description,
            TBU.ATBU_ID,
            JED.AJTB_SeqReferenceNum as status,
			c.cmm_Desc as JeType,JED.ajtb_Masid

        FROM Acc_JETransactions_Details AS JED
        LEFT JOIN Acc_TrailBalance_Upload AS TBU
            ON JED.AJTB_Deschead = TBU.ATBU_ID
LEFT JOIN Acc_JE_Master AS JE   ON JE.ACC_JE_ID = JED.ajtb_masid
		LEFT JOIN Content_Management_Master as c on c.cmm_ID = JE.Acc_JE_BillType and c.cmm_Category = 'JE'

        WHERE 
            TBU.ATBU_DELFLG = 'S'
            AND JED.AJTB_CustId = @CustId
            AND JED.AJTB_YearId = @YearId
            AND JED.AJTB_BranchId = @BranchId AND JED.AJTB_Status <> 'D'
        ORDER BY       JED.AJTB_DescName;";

            // ✅ Step 4: Execute query with parameters
            return await connection.QueryAsync<JournalEntryWithTrailBalanceDto>(query, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId
            });
        }

        //UpdateJESeqReferenceNum
        public async Task<int> UpdateJournalEntrySeqRefAsync(List<UpdateJournalEntrySeqRefDto> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
                return 0; // Nothing to update

            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 3: Build dynamic CASE statement
                var caseStatements = dtoList
                    .Select((d, i) => $"WHEN @Id{i} THEN @SeqReferenceNum{i}")
                    .ToList();

                var sql = $@"
        UPDATE Acc_JETransactions_Details
        SET AJTB_SeqReferenceNum = CASE AJTB_ID
            {string.Join(" ", caseStatements)}
        END
        WHERE AJTB_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Step 4: Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"SeqReferenceNum{i}", dtoList[i].SeqReferenceNum);
                }

                // Step 5: Execute query
                var updatedCount = await connection.ExecuteAsync(sql, parameters, transaction);

                transaction.Commit();
                return updatedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}