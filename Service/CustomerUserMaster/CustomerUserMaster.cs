using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Dto.CustomerUserMaster;
using TracePca.Interface.CustomerUserMaster;

namespace TracePca.Service.CustomerUserMaster
{
    public class CustomerUserMaster: CustomerUserMasterInterface
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CustomerUserMaster(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<CustomerUsersDetailsDto>> GetAllUserDetailsAsync(int companyId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
SELECT 
    a.usr_id AS UserId,
    a.usr_Code AS EmpCode,
    a.usr_FullName AS EmployeeName,
    a.Usr_LoginName AS LoginName,
    a.usr_Email AS Email,
    ISNULL(FORMAT(a.USR_LastLoginDate, 'dd-MMM-yy'), '') AS LastLoginDate,
    ISNULL(z.Cust_Name, '') AS CustomerName,
    CASE a.Usr_DutyStatus
        WHEN 'A' THEN 'Activated'
        WHEN 'D' THEN 'De-Activated'
        WHEN 'W' THEN 'Waiting for Approval'
        ELSE 'Unknown'
    END AS Status
FROM Sad_UserDetails a
LEFT JOIN SAD_CUSTOMER_MASTER z ON z.Cust_ID = a.Usr_CompanyId
WHERE a.Usr_CompID = @CompanyId and usr_type='C'
ORDER BY a.usr_id";



            return await connection.QueryAsync<CustomerUsersDetailsDto>(query, new { CompanyId = companyId });
        }


    }
}
