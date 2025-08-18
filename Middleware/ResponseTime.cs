using System.Diagnostics;
using System.Security.Claims;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TracePca.Middleware
{
    public class ResponseTime
    {
        private readonly RequestDelegate _next;
        private readonly string _connectionString;

        public ResponseTime(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip logging for metrics API itself
            if (path.StartsWith("/api/ApiPerformance", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Record start time
            var stopwatch = Stopwatch.StartNew();

            await _next(context);  // Call the next middleware / controller

            stopwatch.Stop();
            var responseTimeMs = stopwatch.ElapsedMilliseconds;

            // Read FormName from request headers (frontend must send it)
            string formName = context.Request.Headers["FormName"];
            string apiName = context.Request.Path;

            // Get UserId from JWT claims
            int? userId = null;
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            // Optional: add message if response exceeds threshold
            string message = null;
            long thresholdMs = 2000; // example threshold, or use _globalThresholdMs
            if (responseTimeMs > thresholdMs)
            {
                message = $"API exceeded threshold: {thresholdMs} ms";
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(
                    @"INSERT INTO ApiResponseLogs (UserId, FormName, ApiName, ResponseTime, ResponseMessage, CreatedOn)
              VALUES (@UserId, @FormName, @ApiName, @ResponseTime, @ResponseMessage, GETDATE())",
                    new
                    {
                        UserId = userId,
                        FormName = string.IsNullOrEmpty(formName) ? apiName : formName,
                        ApiName = apiName,
                        ResponseTime = responseTimeMs,
                        ResponseMessage  = message
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log API response time: {ex.Message}");
            }
        }


    }
}
