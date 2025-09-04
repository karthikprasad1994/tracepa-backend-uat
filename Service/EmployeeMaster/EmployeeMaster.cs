using Dapper;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using TracePca.Dto.Audit;
using TracePca.Dto.EmployeeMaster;
using TracePca.Interface.EmployeeMaster;

namespace TracePca.Service.EmployeeMaster
{
    public class EmployeeMaster: EmployeeMasterInterface
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public EmployeeMaster(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // Service Layer
        public async Task<IEnumerable<Dto.EmployeeMaster.EmployeesDto>> GetUserFullNameAsync()
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
        SELECT usr_Id AS UserId,
               usr_FullName AS UserName
        FROM Sad_UserDetails";

            return await connection.QueryAsync<Dto.EmployeeMaster.EmployeesDto>(query);

        }

        public async Task<IEnumerable<EmployeeDetailsDto>> GetEmployeeDetailsAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
    SELECT
        u.usr_Id AS EmployeeId,
        u.usr_Code AS EmployeeCode,
        u.usr_FullName AS EmployeeName,
        u.usr_Email AS UserName,
        FORMAT(u.USR_LastLoginDate, 'yyyy-MM-dd') AS LastLoginDate,
        g.Mas_Description AS Role,
        CASE u.Usr_DutyStatus
            WHEN 'W' THEN 'Waiting for Approval'
            WHEN 'D' THEN 'De-Activated'
            WHEN 'A' THEN 'Activated'
            WHEN 'L' THEN 'Lock'
            WHEN 'B' THEN 'Block'
            ELSE 'Unknown'
        END AS Status
    FROM Sad_UserDetails u
    LEFT JOIN SAD_GrpOrLvl_General_Master g ON u.Usr_Role = g.Mas_ID
    WHERE u.Usr_CompId = @CompanyId";

            return await connection.QueryAsync<EmployeeDetailsDto>(query, new { CompanyId = companyId });
        }


        public async Task<IEnumerable<RolesDto>> GetRolesAsync()
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
        SELECT Mas_ID AS RoleId, Mas_Description AS RoleName
        FROM SAD_GrpOrLvl_General_Master"; 

      return await connection.QueryAsync<RolesDto>(query);
        }




    }
}
