using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Dto.CustomerMaster;
using TracePca.Interface;
using TracePca.Interface.EmployeeMaster;

namespace TracePca.Service.CustomerMaster
{
    public class CustomerMaster: CustomerMasterInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CustomerMaster(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<CustomerDetailsDto>> GetCustomersWithStatusAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
SELECT 
    CUST_ID AS CustId,
    CUST_NAME AS CustName,
    CASE CUST_Delflg
        WHEN 'A' THEN 'Activated'
        WHEN 'D' THEN 'De-Activated'
        WHEN 'W' THEN 'Waiting for Approval'
        ELSE 'Unknown'
    END AS Status
FROM SAD_CUSTOMER_MASTER
WHERE CUST_CompID = @CompanyId
ORDER BY CUST_ID";


            return await connection.QueryAsync<CustomerDetailsDto>(query, new { CompanyId = companyId });
        }




    }
}
