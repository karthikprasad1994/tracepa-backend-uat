using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.Middleware;
using TracePca.Interface.Middleware;

namespace TracePca.Service.Miidleware
{
    public class ErrorLoggerService : ErrorLoggerInterface
    {
        private readonly ILogger<ErrorLoggerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public ErrorLoggerService(ILogger<ErrorLoggerService> logger, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public async Task LogErrorAsync(ErrorLogDto dto)
        {
            try
            {
                const string query = @"
                    INSERT INTO ApplicationErrorLogs
                    (
                        FormName, Controller, Action, ErrorMessage,
                        StackTrace, UserId, CustomerId,
                        Description, CreatedDate, ResponseTime
                    )
                    VALUES
                    (
                        @FormName, @Controller, @Action, @ErrorMessage,
                        @StackTrace, @UserId, @CustomerId,
                        @Description, @CreatedDate, @ResponseTime
                    );";

                string dbName = _contextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Get the connection string
                var connectionString = _configuration.GetConnectionString(dbName);

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                //  using var connection = new SqlConnection(_configuration.GetConnectionString("YourConnection"));
                await connection.ExecuteAsync(query, dto);

                _logger.LogInformation("Error logged to database successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log error to database.");



            }





        }

        public async Task<IEnumerable<ErrorLogDto>> GetAllLogsAsync()
        {

            string dbName = _contextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            const string query = @"SELECT * FROM  ApplicationErrorLogs ORDER BY CreatedDate DESC";
           // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryAsync<ErrorLogDto>(query);
        }

        public async Task<ErrorLogDto?> GetLogByIdAsync(int id)
        {

            string dbName = _contextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            const string query = @"SELECT * FROM  ApplicationErrorLogs WHERE Id = @Id";
          //  using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryFirstOrDefaultAsync<ErrorLogDto>(query, new { Id = id });
        }

    }
}
