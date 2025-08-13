using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.Middleware;
using TracePca.Interface.Middleware;

namespace TracePca.Service.Miidleware
{
    public class PerformanceService : PerformanceInterface
    {
        //private readonly string _connectionString;
        private readonly int _globalThresholdMs;
        private readonly IConfiguration _configuration;
        // private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(IConfiguration configuration)
        {
            //_connectionString = configuration.GetConnectionString("DefaultConnection");
            _globalThresholdMs = configuration.GetValue<int?>("ApiPerformance:GlobalThresholdMs") ?? 500;
            _configuration = configuration;
            
        }

        public async Task<IEnumerable<ApiSummaryDto>> GetFormPerformanceSummariesAsync(int lookbackMinutes = 10)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var sql = @"
WITH Aggregated AS (
    SELECT FormName, ApiName,
           AVG(CAST(ResponseTime AS FLOAT)) AS AvgResponseTimeMs,
   MAX(ResponseTime) AS MaxResponseTimeMs,
           MAX(CreatedOn) AS LastSeen
    FROM ApiResponseLogs
    WHERE CreatedOn >= DATEADD(MINUTE, -@LookbackMinutes, GETDATE())
    GROUP BY FormName, ApiName
)
SELECT a.FormName,
       a.ApiName,
       a.AvgResponseTimeMs,
       a.MaxResponseTimeMs,
       latest.ResponseTime AS LatestResponseTimeMs,
       a.LastSeen
FROM Aggregated a
CROSS APPLY (
    SELECT TOP 1 ResponseTime
    FROM ApiResponseLogs l
    WHERE l.FormName = a.FormName
      AND l.ApiName = a.ApiName
      AND l.CreatedOn >= DATEADD(MINUTE, -@LookbackMinutes, GETDATE())
    ORDER BY l.CreatedOn DESC
) latest;";

            try
            {
                return await connection.QueryAsync<ApiSummaryDto>(
                    sql,
                    new { LookbackMinutes = lookbackMinutes }
                );
            }
            catch (Exception ex)
            {
                throw;
               
                // Re-throw so API returns real error in dev
            }
        }

        public async Task<IEnumerable<ApiAlertDto>> GetSlowApisAsync(int lookbackMinutes = 10)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var summaries = (await GetFormPerformanceSummariesAsync(lookbackMinutes)).ToList();

                return summaries
                    .Where(s => s.LatestResponseTimeMs > _globalThresholdMs)
                    .Select(s => new ApiAlertDto
                    {
                        FormName = s.FormName,
                        ApiName = s.ApiName,
                        LatestResponseTimeMs = (int)s.LatestResponseTimeMs, // Explicit cast
                        AvgResponseTimeMs = s.AvgResponseTimeMs,
                        Message = "Taking more response time than average"
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
