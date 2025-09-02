using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleAccountingRatioDto;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleAccountingRatioService : ScheduleAccountingRatioInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScheduleAccountingRatioService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetAccoutingRatio
        public async Task<AssetsLiabilitiesDto> GetAccountingRatioAsync(int yearId, int customerId)
        {
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
                // ✅ Hardcoded schedule type = 4 (same as VB code)
                int schedType = 4;

                // ✅ Get heading ids
                int headingId1 = await GetHeadingId(connection, transaction, customerId, "4 Current Liabilities");
                int headingId2 = await GetHeadingId(connection, transaction, customerId, "2 Current Assets");

                // ✅ Get amounts
                var liabilities = await GetHeadingAmt1(connection, transaction, yearId, customerId, schedType, headingId1);
                var assets = await GetHeadingAmt1(connection, transaction, yearId, customerId, schedType, headingId2);

                transaction.Commit();

                return new AssetsLiabilitiesDto
                {
                    LiabilitiesDc1 = liabilities.Dc1,
                    LiabilitiesDP1 = liabilities.DP1,
                    AssetsDc1 = assets.Dc1,
                    AssetsDP1 = assets.DP1
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private async Task<int> GetHeadingId(SqlConnection connection, SqlTransaction transaction, int customerId, string headName)
        {
            const string sql = @"
        SELECT ASH_ID 
        FROM ACC_ScheduleHeading 
        WHERE ASH_Name = @HeadName 
          AND ASH_OrgType = @CustId";

            return await connection.ExecuteScalarAsync<int?>(
                sql,
                new { HeadName = headName, CustId = customerId },
                transaction
            ) ?? 0;
        }
        // ✅ Get Calculated Amounts
        private async Task<(decimal Dc1, decimal DP1)> GetHeadingAmt1( SqlConnection connection, SqlTransaction transaction,
        int yearId, int customerId, int schedType, int headingId)
        {
            const string getAmtQuery = @"
                SELECT 
                    ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + 0), 0) - 
                        ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount + 0), 0)) AS Dc1,
                    ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount + 0), 0) - 
                        ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount + 0), 0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details
                LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = ATBUD_HeadingId
                LEFT JOIN Acc_TrailBalance_Upload d 
                    ON d.ATBU_Description = ATBUD_Description
                   AND d.ATBU_YearId = @YearId 
                   AND d.ATBU_CustId = @CustomerId 
                   AND ATBUD_YearId = @YearId
                LEFT JOIN Acc_TrailBalance_Upload e 
                    ON e.ATBU_Description = ATBUD_Description
                   AND e.ATBU_YearId = (@YearId - 1) 
                   AND e.ATBU_CustId = @CustomerId 
                   AND ATBUD_YearId = (@YearId - 1)
                WHERE ATBUD_Schedule_Type = @SchedType
                  AND ATBUD_CustId = @CustomerId
                  AND ATBUD_HeadingId = @HeadingId
                GROUP BY ATBUD_HeadingId
                ORDER BY ATBUD_HeadingId";

            //// ✅ Liabilities
            //var liabilities = (headingId1 == 0)
            //    ? (Dc1: 0m, DP1: 0m)
            //    : await connection.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(
            //        getAmtQuery,
            //        new { YearId = yearId, CustomerId = customerId, SchedType = schedType, HeadingId = headingId1 },
            //        transaction);

            //// ✅ Assets
            //var assets = (headingId2 == 0)
            //    ? (Dc1: 0m, DP1: 0m)
            //    : await connection.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(
            //        getAmtQuery,
            //        new { YearId = yearId, CustomerId = customerId, SchedType = schedType, HeadingId = headingId2 },
            //        transaction);

            return (headingId == 0)
         ? (Dc1: 0m, DP1: 0m)
         : await connection.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(
             getAmtQuery,
             new { YearId = yearId, CustomerId = customerId, SchedType = schedType, HeadingId = headingId },
             transaction);
        }
    }
}
