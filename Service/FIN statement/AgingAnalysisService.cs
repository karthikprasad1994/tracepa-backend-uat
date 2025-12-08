using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.AgingAnalysisDto;

namespace TracePca.Service.FIN_statement
{
    public class AgingAnalysisService : AgingAnalysisInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AgingAnalysisService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetAnalysisBasedOnMonthForTradePayables
        public async Task<IEnumerable<TradePayablesDto>> GetTradePayablesAsync(
       int CompId, int CustId, int BranchId, int YearId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Get SubHeadingId
            var subHeadingId = await connection.ExecuteScalarAsync<int>(
                @"SELECT assh_id 
          FROM ACC_ScheduleSubHeading 
          WHERE assh_name='(b) Trade payables' 
            AND ASSH_OrgType = @Custid",
                new { Custid = CustId });

            // Step 2: Get all related ATBUD IDs
            var transIds = (await connection.QueryAsync<int>(
                @"SELECT atbud_masid 
          FROM Acc_TrailBalance_Upload_Details 
          WHERE ATBUD_Subheading = @subHeadingId 
            AND ATBUD_YearId = @Yearid",
                new { subHeadingId, Yearid = YearId }
            )).ToList();

            if (!transIds.Any())
                return Enumerable.Empty<TradePayablesDto>();

            // Step 3: Fetch JE Transactions
            var query = @"
SELECT 
    AJTB_Deschead as ID,
    AJTB_DescName as AJTB_DescName,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Debit ELSE 0 END) AS Debit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Debit ELSE 0 END) AS Debit_After6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Credit ELSE 0 END) AS Credit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Credit ELSE 0 END) AS Credit_After6M,
    SUM(AJTB_Debit) AS Total_Debit,
    SUM(AJTB_Credit) AS Total_Credit
FROM Acc_JETransactions_Details
WHERE AJTB_Deschead IN @TransIds
  AND AJTB_CustId = @CustId 
  AND AJTB_YearID = @YearId  
  AND AJTB_CompId = @CompId  
  AND AJTB_BranchId = @BranchId
GROUP BY AJTB_Deschead, AJTB_DescName
ORDER BY AJTB_Deschead;";

            // Step 4: Execute query
            var result = await connection.QueryAsync<TradePayablesDto>(
                query,
                new
                {
                    CompId,
                    CustId,
                    BranchId,
                    YearId,
                    TransIds = transIds
                });

            return result;
        }




        //GetAnalysisBasedOnMonthForTradeReceivables
        public async Task<IEnumerable<TradePayablesDto>> GetTradeReceiveablesAsync(int CompId, int CustId, int BranchId, int YearId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Get SubHeadingId
            var subHeadingId = await connection.ExecuteScalarAsync<int>(
                @"SELECT assh_id 
          FROM ACC_ScheduleSubHeading 
          WHERE assh_name='(c) Trade receivables' 
            AND ASSH_OrgType = @Custid",
                new { Custid = CustId });

            // Step 2: Get all related ATBUD IDs
            var transIds = (await connection.QueryAsync<int>(
                @"SELECT atbud_masid 
          FROM Acc_TrailBalance_Upload_Details 
          WHERE ATBUD_Subheading = @subHeadingId 
            AND ATBUD_YearId = @Yearid",
                new { subHeadingId, Yearid = YearId }
            )).ToList();

            if (!transIds.Any())
                return Enumerable.Empty<TradePayablesDto>();

            // Step 3: Fetch JE Transactions
            var query = @"
SELECT 
    AJTB_Deschead as ID,
    AJTB_DescName as AJTB_DescName,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Debit ELSE 0 END) AS Debit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Debit ELSE 0 END) AS Debit_After6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Credit ELSE 0 END) AS Credit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Credit ELSE 0 END) AS Credit_After6M,
    SUM(AJTB_Debit) AS Total_Debit,
    SUM(AJTB_Credit) AS Total_Credit
FROM Acc_JETransactions_Details
WHERE AJTB_Deschead IN @TransIds
  AND AJTB_CustId = @CustId 
  AND AJTB_YearID = @YearId  
  AND AJTB_CompId = @CompId  
  AND AJTB_BranchId = @BranchId
GROUP BY AJTB_Deschead, AJTB_DescName
ORDER BY AJTB_Deschead;";

            // Step 4: Execute query
            var result = await connection.QueryAsync<TradePayablesDto>(
                query,
                new
                {
                    CompId,
                    CustId,
                    BranchId,
                    YearId,
                    TransIds = transIds
                });

            return result;
        }

        //GetAnalysisBasedOnMonthForTradePayablesById
        public async Task<IEnumerable<TradePayablesByIdDto>> GetTradePayablesByIdAsync(
       int CompId, int CustId, int BranchId, int YearId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Get SubHeadingId
            var subHeadingId = await connection.ExecuteScalarAsync<int>(
                @"SELECT assh_id 
          FROM ACC_ScheduleSubHeading 
          WHERE assh_name='(b) Trade payables' 
            AND ASSH_OrgType = @Custid",
                new { Custid = CustId });

            // Step 2: Get all related ATBUD IDs
            var transIds = (await connection.QueryAsync<int>(
                @"SELECT atbud_id 
          FROM Acc_TrailBalance_Upload_Details 
          WHERE ATBUD_Subheading = @subHeadingId 
            AND ATBUD_YearId = @Yearid",
                new { subHeadingId, Yearid = YearId }
            )).ToList();

            if (!transIds.Any())
                return Enumerable.Empty<TradePayablesByIdDto>();

            // Step 3: Fetch JE Transactions
            var query = @"
SELECT 
    AJTB_Deschead as ID,
    AJTB_DescName as AJTB_DescName,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Debit ELSE 0 END) AS Debit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Debit ELSE 0 END) AS Debit_After6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Credit ELSE 0 END) AS Credit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Credit ELSE 0 END) AS Credit_After6M,
    SUM(AJTB_Debit) AS Total_Debit,
    SUM(AJTB_Credit) AS Total_Credit
FROM Acc_JETransactions_Details
WHERE AJTB_Deschead IN @TransIds
  AND AJTB_CustId = @CustId 
  AND AJTB_YearID = @YearId  
  AND AJTB_CompId = @CompId  
  AND AJTB_BranchId = @BranchId
GROUP BY AJTB_ID, AJTB_Deschead, AJTB_DescName
ORDER BY AJTB_ID, AJTB_Deschead;";

            // Step 4: Execute query
            var result = await connection.QueryAsync<TradePayablesByIdDto>(
                query,
                new
                {
                    CompId,
                    CustId,
                    BranchId,
                    YearId,
                    TransIds = transIds
                }
            );

            return result;
        }

        //GetAnalysisBasedOnMonthForTradeReceivablesById
        public async Task<IEnumerable<TradeReceiveablesByIdDto>> GetTradeReceiveablesByIdAsync(int CompId, int CustId, int BranchId, int YearId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Get SubHeadingId
            var subHeadingId = await connection.ExecuteScalarAsync<int>(
                @"SELECT assh_id 
          FROM ACC_ScheduleSubHeading 
          WHERE assh_name='(c) Trade receivables' 
            AND ASSH_OrgType = @Custid",
                new { Custid = CustId });

            // Step 2: Get all related ATBUD IDs
            var transIds = (await connection.QueryAsync<int>(
                @"SELECT atbud_id 
          FROM Acc_TrailBalance_Upload_Details 
          WHERE ATBUD_Subheading = @subHeadingId 
            AND ATBUD_YearId = @Yearid",
                new { subHeadingId, Yearid = YearId }
            )).ToList();

            if (!transIds.Any())
                return Enumerable.Empty<TradeReceiveablesByIdDto>();

            // Step 3: Fetch JE Transactions
            var query = @"
SELECT 
    AJTB_Deschead as ID,
    AJTB_DescName as AJTB_DescName,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Debit ELSE 0 END) AS Debit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Debit ELSE 0 END) AS Debit_After6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 4 AND 9 THEN AJTB_Credit ELSE 0 END) AS Credit_Before6M,
    SUM(CASE WHEN MONTH(AJTB_CreatedOn) BETWEEN 10 AND 12 OR MONTH(AJTB_CreatedOn) BETWEEN 1 AND 3 THEN AJTB_Credit ELSE 0 END) AS Credit_After6M,
    SUM(AJTB_Debit) AS Total_Debit,
    SUM(AJTB_Credit) AS Total_Credit
FROM Acc_JETransactions_Details
WHERE AJTB_Deschead IN @TransIds
  AND AJTB_CustId = @CustId 
  AND AJTB_YearID = @YearId  
  AND AJTB_CompId = @CompId  
  AND AJTB_BranchId = @BranchId
GROUP BY AJTB_ID, AJTB_Deschead, AJTB_DescName
ORDER BY AJTB_Id, AJTB_Deschead;";

            // Step 4: Execute query
            var result = await connection.QueryAsync<TradeReceiveablesByIdDto>(
                query,
                new
                {
                    CompId,
                    CustId,
                    BranchId,
                    YearId,
                    TransIds = transIds
                }
            );
            return result;
        }
    }
}
