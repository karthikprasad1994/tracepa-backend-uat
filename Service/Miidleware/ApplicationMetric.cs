using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Dto.Application_Metric;
using TracePca.Interface.Middleware;

namespace TracePca.Service.Miidleware
{
    public class ApplicationMetric : ApplicationMetricInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public ApplicationMetric(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApplicationMetricDto> GetApplicationMetricsAsync()
        {

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            //string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            //if (string.IsNullOrEmpty(dbName))
            //    throw new Exception("CustomerCode is missing in session. Please log in again.");

            //var connectionString = _configuration.GetConnectionString(dbName);
            //using var connection = new SqlConnection(connectionString);
            //await connection.OpenAsync();

            // 1. Avg Load Time by Form
            var avgLoadTime = (await connection.QueryAsync<AvgLoadTimeDto>(
                @"SELECT FormName, AVG(ResponseTime) AS AvgLoadTime
              FROM ApplicationErrorLogs
              WHERE ResponseTime IS NOT NULL AND FormName IS NOT NULL
              GROUP BY FormName")).ToList();
            
            var activeUserCount = await connection.ExecuteScalarAsync<int>(
           @"SELECT COUNT(DISTINCT UserId)
              FROM ApplicationErrorLogs
              WHERE CreatedDate >= DATEADD(MINUTE, -30, GETUTCDATE())");

            var totalUserCount = await connection.ExecuteScalarAsync<int>(
         @"SELECT COUNT(DISTINCT UserId) FROM ApplicationErrorLogs");

            var userActivity = new UserActivityStatusDto
            {
                ActiveUsers = activeUserCount,
                InactiveUsers = totalUserCount - activeUserCount
            };

            var requestThroughput = (await connection.QueryAsync<RequestThroughputDto>(
            @"SELECT FORMAT(DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate)/15*15, 0), 'HH:mm') AS TimeSlot,
                     COUNT(*) AS RequestCount
              FROM ApplicationErrorLogs
              WHERE CreatedDate >= DATEADD(HOUR, -1, GETUTCDATE())
              GROUP BY FORMAT(DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate)/15*15, 0), 'HH:mm')
              ORDER BY TimeSlot")).ToList();

            var summaryStats = await connection.QueryFirstOrDefaultAsync<SummaryStatsDto>(
            @"SELECT 
                ISNULL(AVG(ResponseTime), 0) AS AvgResponseTimeMs,
                CAST(100.0 * SUM(CASE WHEN ErrorMessage IS NOT NULL THEN 1 ELSE 0 END) / NULLIF(COUNT(*), 0) AS FLOAT) AS ErrorRatePercent,
                COUNT(DISTINCT CASE WHEN CreatedDate >= DATEADD(MINUTE, -30, GETUTCDATE()) THEN UserId END) AS ActiveUserCount
              FROM ApplicationErrorLogs");

            var errorBreakdown = (await connection.QueryAsync<ErrorBreakdownDto>(
            @"SELECT LEFT(ErrorMessage, CHARINDEX(':', ErrorMessage + ':') - 1) AS ErrorType,
                     COUNT(*) AS Count
              FROM ApplicationErrorLogs
              WHERE ErrorMessage IS NOT NULL
              GROUP BY LEFT(ErrorMessage, CHARINDEX(':', ErrorMessage + ':') - 1)")).ToList();

            return new ApplicationMetricDto
            {
                AvgLoadTimeByForm = avgLoadTime,
                UserActivityStatus = userActivity,
                RequestThroughput = requestThroughput,
                SummaryStats = summaryStats,
                ErrorBreakdown = errorBreakdown
            };

        }
    }
}

