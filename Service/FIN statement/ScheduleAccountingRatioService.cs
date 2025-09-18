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
        public async Task<Ratio1Dto> GetAccountingRatioAsync(int yearId, int customerId)
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

                return new Ratio1Dto
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
        private async Task<(decimal Dc1, decimal DP1)> GetHeadingAmt1(SqlConnection connection, SqlTransaction transaction,
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

            return (headingId == 0)
         ? (Dc1: 0m, DP1: 0m)
         : await connection.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(
             getAmtQuery,
             new { YearId = yearId, CustomerId = customerId, SchedType = schedType, HeadingId = headingId },
             transaction);
        }

        //GetAccoutingRatio2
        public async Task<Ratio2Dto> GetBorrowingsVsShareholdersAsync(int yearId, int customerId, int branchId)
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
                int schedType = 4;

                // ✅ SubHeading IDs
                int subHeadingId1 = await GetSubHeadingId(connection, transaction, customerId, "(a) Long-term borrowings");
                int subHeadingId2 = await GetSubHeadingId(connection, transaction, customerId, "(a) Short Term Borrowings");

                // ✅ Heading ID
                int headingId1 = await GetHeadingIdd(connection, transaction, customerId, "1 Shareholders Funds");

                // ✅ Amounts
                var longTermBorrower = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, schedType, subHeadingId1);
                var shortTermBorrower = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, schedType, subHeadingId2);
                var shareHolderFund = await GetHeadingAmt1S(connection, transaction, yearId, customerId, schedType, headingId1);

                var dPandLamt = await GetPandLFinalAmt(connection, transaction, yearId, customerId, branchId);
                var dPrevPandLamt = await GetPandLFinalAmt(connection, transaction, yearId - 1, customerId, branchId);

                // ✅ Calculations
                double dCTotal1 = 0, dPTotal1 = 0, dCTotal2 = 0, dPTotal2 = 0, dDiff = 0;

                if (longTermBorrower.Dc1 != 0 || shortTermBorrower.Dc1 != 0)
                    dCTotal1 = (double)(longTermBorrower.Dc1 + shortTermBorrower.Dc1);

                if (longTermBorrower.DP1 != 0 || shortTermBorrower.DP1 != 0)
                    dPTotal1 = (double)(longTermBorrower.DP1 + shortTermBorrower.DP1);

                if (shareHolderFund.Dc1 != 0 || dCTotal1 != 0)
                    dCTotal2 = dCTotal1 / (double)(shareHolderFund.Dc1 + dPandLamt);

                if (shareHolderFund.DP1 != 0 || dPTotal1 != 0)
                    dPTotal2 = dPTotal1 / (double)(shareHolderFund.DP1 + dPrevPandLamt);

                dDiff = dCTotal2 - dPTotal2;

                // Handle NaN/Infinity
                if (double.IsNaN(dCTotal2)) dCTotal2 = 0;
                if (double.IsNaN(dPTotal2)) dPTotal2 = 0;
                if (double.IsNaN(dDiff)) dDiff = 0;
                if (double.IsInfinity(dCTotal2)) dCTotal2 = 0;
                if (double.IsInfinity(dPTotal2)) dPTotal2 = 0;

                transaction.Commit();

                return new Ratio2Dto
                {
                    Current_Reporting_Period = Math.Round((decimal)dCTotal2, 2),
                    Previous_Reporting_Period = Math.Round((decimal)dPTotal2, 2),
                    Change = Math.Round((decimal)dDiff, 2)
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private async Task<int> GetSubHeadingId(SqlConnection connection, SqlTransaction transaction, int customerId, string headName)
        {
            const string sql = @"
        SELECT ASSH_ID 
        FROM ACC_ScheduleSubHeading 
        WHERE ASSH_Name = @HeadName 
          AND ASSH_OrgType = @CustId";

            return await connection.ExecuteScalarAsync<int?>(
                sql,
                new { HeadName = headName, CustId = customerId },
                transaction
            ) ?? 0;
        }
        private async Task<int> GetHeadingIdd(SqlConnection connection, SqlTransaction transaction, int customerId, string headName)
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
        private async Task<(decimal Dc1, decimal DP1)> GetSubHeadingAmt1(SqlConnection connection, SqlTransaction transaction,
            int yearId, int customerId, int schedType, int subHeadingId)
        {
            const string getAmtQuery = @"
        SELECT 
            ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount + 0), 0) -
                ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount + 0), 0)) AS Dc1,
            ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount + 0), 0) -
                ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount + 0), 0)) AS DP1
        FROM Acc_TrailBalance_Upload_Details
        LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
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
          AND ATBUD_Subheading = @SubHeadingId
        GROUP BY ATBUD_HeadingId
        ORDER BY ATBUD_HeadingId";

            return (subHeadingId == 0)
                ? (Dc1: 0m, DP1: 0m)
                : await connection.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(
                    getAmtQuery,
                    new { YearId = yearId, CustomerId = customerId, SchedType = schedType, SubHeadingId = subHeadingId },
                    transaction);
        }
        private async Task<(decimal Dc1, decimal DP1)> GetHeadingAmt1S(SqlConnection connection, SqlTransaction transaction,
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

            return (headingId == 0)
                ? (Dc1: 0m, DP1: 0m)
                : await connection.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(
                    getAmtQuery,
                    new { YearId = yearId, CustomerId = customerId, SchedType = schedType, HeadingId = headingId },
                    transaction);
        }
        private async Task<decimal> GetPandLFinalAmt(SqlConnection connection, SqlTransaction transaction,
            int yearId, int customerId, int branchId)
        {
            const string sql = @"
        SELECT SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount) AS Amt
        FROM Acc_TrailBalance_Upload
        WHERE ATBU_Description = 'Net Income'
          AND ATBU_YearId = @YearId
          AND ATBU_CustId = @CustomerId
          AND ATBU_BranchId = @BranchId";

            return await connection.ExecuteScalarAsync<decimal?>(
                sql,
                new { YearId = yearId, CustomerId = customerId, BranchId = branchId },
                transaction
            ) ?? 0m;
        }
    }
}