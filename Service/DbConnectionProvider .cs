using Microsoft.Data.SqlClient;
using TracePca.Interface.DatabaseConnection;

namespace TracePca.Service
{
    public class DbConnectionProvider : DbConnectionInterface
    {
        private readonly IConfiguration _configuration;
        private string _customerCode;

        public DbConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SetCustomerCode(string customerCode)
        {
            _customerCode = customerCode;
        }

        public SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_customerCode))
                throw new InvalidOperationException("CustomerCode is not set.");

            var connStr = _configuration.GetConnectionString(_customerCode);
            return new SqlConnection(connStr);
        }
    }
}
