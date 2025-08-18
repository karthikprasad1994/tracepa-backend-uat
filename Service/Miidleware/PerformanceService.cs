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

        //        public async Task<IEnumerable<ApiSummaryDto>> GetFormPerformanceSummariesAsync(int lookbackMinutes = 10)
        //        {
        //            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //            var sql = @"
        //WITH Aggregated AS (
        //SELECT FormName, ApiName,
        //           AVG(CAST(ResponseTime AS FLOAT)) AS AvgResponseTimeMs,
        //   MAX(ResponseTime) AS MaxResponseTimeMs,
        //           MAX(CreatedOn) AS LastSeen
        //    FROM ApiResponseLogs
        //    WHERE CreatedOn >= DATEADD(MINUTE, -@LookbackMinutes, GETDATE())
        //    GROUP BY FormName, ApiName
        //)
        //SELECT a.FormName,
        //       a.ApiName,
        //       a.AvgResponseTimeMs,
        //       a.MaxResponseTimeMs,
        //       latest.ResponseTime AS LatestResponseTimeMs,
        //       a.LastSeen
        //FROM Aggregated a
        //CROSS APPLY (
        //    SELECT TOP 1 ResponseTime
        //    FROM ApiResponseLogs l
        //    WHERE l.FormName = a.FormName
        //      AND l.ApiName = a.ApiName
        //      AND l.CreatedOn >= DATEADD(MINUTE, -@LookbackMinutes, GETDATE())
        //    ORDER BY l.CreatedOn DESC
        //) latest;";

        //            try
        //            {
        //                return await connection.QueryAsync<ApiSummaryDto>(
        //                    sql,
        //                    new { LookbackMinutes = lookbackMinutes }
        //                );
        //            }
        //            catch (Exception ex)
        //            {
        //                throw;

        //                // Re-throw so API returns real error in dev
        //            }
        //        }

        //        public async Task<ApiSummaryWithUsersDto> GetFormPerformanceSummariesAsync(int lookbackMinutes = 10)
        //        {
        //            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //            // === API performance summary query (includes latest response time) ===
        //            var sqlSummary = @"
        //WITH Aggregated AS (
        //    SELECT 
        //        FormName, 
        //        ApiName,
        //        AVG(CAST(ResponseTime AS FLOAT)) AS AvgResponseTimeMs,
        //        MAX(ResponseTime) AS MaxResponseTimeMs,
        //        MAX(CreatedOn) AS LastSeen
        //    FROM ApiResponseLogs
        //    WHERE CreatedOn >= DATEADD(MINUTE, -@LookbackMinutes, GETUTCDATE())
        //    GROUP BY FormName, ApiName
        //)
        //SELECT 
        //    a.FormName,
        //    a.ApiName,
        //    a.AvgResponseTimeMs,
        //    a.MaxResponseTimeMs,
        //    latest.ResponseTime AS LatestResponseTimeMs,
        //    a.LastSeen
        //FROM Aggregated a
        //CROSS APPLY (
        //    SELECT TOP 1 ResponseTime
        //    FROM ApiResponseLogs l
        //    WHERE l.FormName = a.FormName
        //      AND l.ApiName = a.ApiName
        //      AND l.CreatedOn >= DATEADD(MINUTE, -@LookbackMinutes, GETUTCDATE())
        //    ORDER BY l.CreatedOn DESC
        //) latest
        //ORDER BY a.AvgResponseTimeMs DESC;";

        //            // === Active/Inactive user counts ===
        //            var sqlUsers = @"
        //WITH UserStatus AS (
        //    SELECT 
        //        UserId,
        //        CASE 
        //            WHEN MAX(CASE WHEN IsRevoked = 0 AND AccessTokenExpiry > GETUTCDATE() THEN 1 ELSE 0 END) = 1 
        //            THEN 'Active'
        //            ELSE 'Inactive'
        //        END AS Status
        //    FROM UserTokens
        //    GROUP BY UserId
        //)
        //SELECT
        //    SUM(CASE WHEN Status = 'Active' THEN 1 ELSE 0 END) AS ActiveUsers,
        //    SUM(CASE WHEN Status = 'Inactive' THEN 1 ELSE 0 END) AS InactiveUsers
        //FROM UserStatus;";

        //            // Run queries
        //            var summaries = await connection.QueryAsync<ApiSummaryDto>(
        //                sqlSummary,
        //                new { LookbackMinutes = lookbackMinutes }
        //            );

        //            var userCounts = await connection.QuerySingleAsync<(int ActiveUsers, int InactiveUsers)>(sqlUsers);

        //            // Return combined result
        //            return new ApiSummaryWithUsersDto
        //            {
        //                Summaries = summaries.ToList(),
        //                ActiveUsers = userCounts.ActiveUsers,
        //                InactiveUsers = userCounts.InactiveUsers
        //            };
        //        }

        public async Task<List<ApiSummaryDto>> GetFormPerformanceSummariesAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var sql = @"
WITH Aggregated AS (
    SELECT 
        FormName, 
        ApiName,
        AVG(CAST(ResponseTime AS FLOAT)) AS AvgResponseTimeMs,
        MAX(ResponseTime) AS MaxResponseTimeMs,
        MAX(CreatedOn) AS LastSeen
    FROM ApiResponseLogs
    GROUP BY FormName, ApiName
),
UserActivity AS (
    SELECT DISTINCT 
        l.FormName,
        l.ApiName,
        l.UserId
    FROM ApiResponseLogs l
    INNER JOIN UserTokens t ON l.UserId = t.UserId AND t.IsRevoked = 0
)
SELECT 
    a.FormName,
    a.ApiName,
    a.AvgResponseTimeMs,
    a.MaxResponseTimeMs,
    latest.ResponseTime AS LatestResponseTimeMs,
    a.LastSeen,
    COUNT(DISTINCT ua.UserId) AS ActiveUsers,
    (SELECT COUNT(*) FROM UserTokens WHERE IsRevoked = 0) - COUNT(DISTINCT ua.UserId) AS InactiveUsers
FROM Aggregated a
CROSS APPLY (
    SELECT TOP 1 ResponseTime
    FROM ApiResponseLogs l
    WHERE l.FormName = a.FormName
      AND l.ApiName = a.ApiName
    ORDER BY l.CreatedOn DESC
) latest
LEFT JOIN UserActivity ua ON ua.FormName = a.FormName AND ua.ApiName = a.ApiName
GROUP BY 
    a.FormName, a.ApiName, a.AvgResponseTimeMs, a.MaxResponseTimeMs, a.LastSeen, latest.ResponseTime
ORDER BY a.AvgResponseTimeMs DESC;
";

            var summaries = (await connection.QueryAsync<ApiSummaryDto>(sql)).ToList();
            return summaries;
        }



    }





}

