using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace TracePca.Utility
{
    public interface IDbConnectionFactory
    {
        SqlConnection CreateConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbConnectionFactory(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public SqlConnection CreateConnection()
        {
            // ✅ Get dbName from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Replace {DBName} in connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            return new SqlConnection(connectionString);
        }
    }
}
