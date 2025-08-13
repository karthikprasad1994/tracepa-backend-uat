using System.Diagnostics;
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

            // Read formName from request headers (passed from frontend)
            string formName = context.Request.Headers["FormName"];
            string apiName = context.Request.Path;

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(
                    @"INSERT INTO ApiResponseLogs (FormName, ApiName, ResponseTime, CreatedOn)
                      VALUES (@FormName, @ApiName, @ResponseTime, GETDATE())",
                    new
                    {
                        FormName = string.IsNullOrEmpty(formName) ? apiName : formName,
                        ApiName = apiName,
                        ResponseTime = responseTimeMs
                    });
            }
            catch (Exception ex)
            {
                // Optional: log this somewhere instead of throwing
                Console.WriteLine($"Failed to log API response time: {ex.Message}");
            }
        }
    }
}
