using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Dto.FIN_Statement.SchedulePartnerFundsDto;
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
            ATBU_Closing_TotalCredit_Amount, ATBU_Status      
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
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Execute for all records one by one in a transaction
            using var transaction = connection.BeginTransaction();

            try
            {
                int totalUpdated = 0;

                foreach (var dto in dtoList)
                {
                    // ✅ Define query inside the loop
                    var query = @"
            UPDATE Acc_TrailBalance_Upload
            SET ATBU_STATUS = @Status
            WHERE ATBU_ID = @Id;";

                    // ✅ Execute query for each DTO
                    totalUpdated += await connection.ExecuteAsync(query, new
                    {
                        Id = dto.Id,
                        Status = dto.Status
                    }, transaction);
                }

                transaction.Commit();

                return totalUpdated; // ✅ Return total number of records updated
            }
            catch
            {
                transaction.Rollback();
                throw; // Let the caller handle the error
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
            JED.AJTB_Debit,
            JED.AJTB_Credit,
            TBU.ATBU_Description,
            TBU.ATBU_ID
        FROM Acc_JETransactions_Details AS JED
        LEFT JOIN Acc_TrailBalance_Upload AS TBU
            ON JED.AJTB_Deschead = TBU.ATBU_ID
        WHERE 
            TBU.ATBU_STATUS = 'A'
            AND JED.AJTB_CustId = @CustId
            AND JED.AJTB_YearId = @YearId
            AND JED.AJTB_BranchId = @BranchId
        ORDER BY 
            JED.AJTB_ID;";

            // ✅ Step 4: Execute query with parameters
            return await connection.QueryAsync<JournalEntryWithTrailBalanceDto>(query, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId
            });
        }
    }
}