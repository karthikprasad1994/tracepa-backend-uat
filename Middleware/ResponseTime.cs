using System.Diagnostics;
using System.Security.Claims;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TracePca.Middleware
{
    public class ResponseTime
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResponseTime(RequestDelegate next, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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

                // Get UserId from session (set during login)
                int? userId = context.Session.GetInt32("UserId");

                // ✅ Get DB name from session
                string dbName = context.Session.GetString("CustomerCode");
                if (string.IsNullOrEmpty(dbName))
                {
                    Console.WriteLine("CustomerCode is missing in session. Skipping logging.");
                    return;
                }

                // ✅ Resolve connection string dynamically
                var connectionString = _configuration.GetConnectionString(dbName);
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine($"No connection string found for '{dbName}'. Skipping logging.");
                    return;
                }

                // Optional: add message if response exceeds threshold
                string? message = null;
                long thresholdMs = 2000; // example threshold
                if (responseTimeMs > thresholdMs)
                {
                    message = $"API exceeded threshold: {thresholdMs} ms";
                }

                try
                {
                    using var connection = new SqlConnection(connectionString);
                    await connection.ExecuteAsync(
                        @"INSERT INTO ApiResponseLogs 
                      (UserId, FormName, ApiName, ResponseTime, ResponseMessage, CreatedOn)
                      VALUES (@UserId, @FormName, @ApiName, @ResponseTime, @ResponseMessage, GETDATE())",
                        new
                        {
                            UserId = userId,
                            FormName = string.IsNullOrEmpty(formName) ? apiName : formName,
                            ApiName = apiName,
                            ResponseTime = responseTimeMs,
                            ResponseMessage = message
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to log API response time: {ex.Message}");
                }
            }
        }
    }





    

