using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;
using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;
using static TracePca.Dto.FIN_Statement.SamplingDto;

namespace TracePca.Service.FIN_statement
{
    public class SamplingService : SamplingInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SamplingService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetSystemSampling
        public async Task<IEnumerable<SystemstemSamplingDTO>> GetSystemSamplingAsync(
         int compId,
         int custId,
         int branchId,
         int yearId,
         int nthPosition,
         int fromRow,
         int toRow,
         int sampleSize)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
;WITH OrderedTransactions AS (
    SELECT AJTB_ID,AJTB_DescName,AJTB_Debit,
        AJTB_Credit, AJTB_SyStatus,
        ROW_NUMBER() OVER (ORDER BY AJTB_ID) AS RowNum,CAST(AJTB_CreatedOn as date) as TransDate
    FROM Acc_JETransactions_Details
    WHERE AJTB_CustId = @iCustId and ajtb_yearid=@iYearId and AJTB_Status <> 'D')
SELECT TOP (@SampleSize) *
FROM OrderedTransactions
WHERE RowNum BETWEEN @FromRow AND @ToRow
  AND ((RowNum - @FromRow) % @NthPosition = 0)
ORDER BY RowNum;";

            var parameters = new
            {
                iCustId = custId,
                iCompId = compId,
                iBranchId = branchId,
                iYearId = yearId,
                NthPosition = nthPosition,
                FromRow = fromRow,
                ToRow = toRow,
                SampleSize = sampleSize
            };

            return await connection.QueryAsync<SystemstemSamplingDTO>(query, parameters);
        }

        //GetStatifiedSamping
        public async Task<IEnumerable<StratifiedSamplingDTO>> GetStratifiedSamplingAsync(
    int compId, int custId, int branchId, int yearId, decimal percentage)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            // Step 3: SQL Logic for Stratified Sampling
            var query = @"
DECLARE @TotalRows INT;
SELECT @TotalRows = COUNT(*) 
FROM Acc_JETransactions_Details 
WHERE AJTB_CustId = @iCustId AND AJTB_YearId = @iYearId;
DECLARE @SampleCount INT = CEILING(@TotalRows * (@Percentage / 100.0));
;WITH OrderedTransactions AS (
    SELECT AJTB_ID,AJTB_DescName,AJTB_Debit,AJTB_Credit,
        CAST(AJTB_CreatedOn AS date) AS TransDate, AJTB_SfStatus,
        ROW_NUMBER() OVER (ORDER BY AJTB_ID) AS RowNum
    FROM Acc_JETransactions_Details
    WHERE AJTB_CustId = @iCustId 
      AND AJTB_YearId = @iYearId)
SELECT TOP (@SampleCount) *
FROM OrderedTransactions
ORDER BY NEWID(); ";
            var parameters = new
            {
                iCustId = custId,
                iCompId = compId,
                iBranchId = branchId,
                iYearId = yearId,
                Percentage = percentage
            };
            return await connection.QueryAsync<StratifiedSamplingDTO>(query, parameters);
        }

        //UpdateSystemSamplingStatus
        public async Task<int> UpdateSystemSamplingStatusAsync(List<UpdateSystemSamplingStatusDto> dtoList)
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
                    .Select((d, i) => $"WHEN @Id{i} THEN @Status{i}")
                    .ToList();

                var sql = $@"
 UPDATE Acc_JETransactions_Details
 SET AJTB_SyStatus = CASE AJTB_ID
     {string.Join(" ", caseStatements)}
 END
 WHERE AJTB_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Step 4: Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"Status{i}", dtoList[i].Status);
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

        //UpdateStatifiedSampingStatus
        public async Task<int> UpdateStatifiedSampingStatusAsync(List<UpdateStatifiedSampingStatusDto> dtoList)
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
                    .Select((d, i) => $"WHEN @Id{i} THEN @Status{i}")
                    .ToList();

                var sql = $@"
 UPDATE Acc_JETransactions_Details
 SET AJTB_SfStatus = CASE AJTB_ID
     {string.Join(" ", caseStatements)}
 END
 WHERE AJTB_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Step 4: Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"Status{i}", dtoList[i].Status);
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


