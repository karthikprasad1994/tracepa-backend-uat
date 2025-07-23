using Microsoft.Data.SqlClient;
using TracePca.Interface.DatabaseConnection;

namespace TracePca.Service
{
    public class DbConnectionProvider : DbConnectionInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerContext _customerContext;

        
        public DbConnectionProvider(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ICustomerContext customerContext)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _customerContext = customerContext;
        }

        public void SetCustomerCode(string customerCode)
        {
            _customerContext.CustomerCode = customerCode; // Set in shared context
        }

        public SqlConnection GetConnection()
        {
            var customerCode = _customerContext.CustomerCode; // ✅ Corrected casing
            if (string.IsNullOrEmpty(customerCode))
                throw new InvalidOperationException("CustomerCode is not set.");

            var connStr = _configuration.GetConnectionString("DynamicDatabase")
                                        .Replace("{customerCode}", customerCode);

            return new SqlConnection(connStr);
        }
    }
}
