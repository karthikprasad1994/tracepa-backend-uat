using System.Data;
using System.Security.Cryptography;
using System.Text;
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
    CASE 
        WHEN u.USR_LastLoginDate IS NULL THEN NULL
        ELSE FORMAT(u.USR_LastLoginDate, 'yyyy-MM-dd')
    END AS LastLoginDate,
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
WHERE u.Usr_CompId = @CompanyId
ORDER BY u.usr_Id"; // ✅ Added ORDER BY


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

        public async Task<string> SaveEmployeeBasicDetailsAsync(EmployeeBasicDetailsDto dto)
        {
            // Confirm Password check
            if (dto.Password != dto.ConfirmPassword)
            {
                return "Password and Confirm Password do not match.";
            }

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            var parameters = new DynamicParameters();

            // Insert (new user, UserId = 0) or Update (existing user, UserId > 0)
            parameters.Add("@Usr_ID", dto.UserId ?? 0);
            parameters.Add("@Usr_Status", dto.UserId == 0 ? "U" : "C");


            // Required fields (mapped from DTO)
            parameters.Add("@Usr_FullName", dto.EmployeeName);
            parameters.Add("@Usr_LoginName", dto.LoginName);
            parameters.Add("@Usr_Password", EncryptPassword(dto.Password));
            parameters.Add("@Usr_Email", dto.Email);
            parameters.Add("@Usr_MobileNo", dto.MobileNo);
            parameters.Add("@Usr_Role", dto.RoleId);

            // Additional SP params (set from DTO if present, else default values)
            // Additional required parameters (hardcoded/default values)
            parameters.Add("@Usr_Node", 0);
            parameters.Add("@Usr_Code", dto.EmpCode);
            parameters.Add("@Usr_Category", 0);
            parameters.Add("@Usr_Suggetions", 0);
            parameters.Add("@usr_partner", 0);
            parameters.Add("@Usr_LevelGrp", 0);
            parameters.Add("@Usr_DutyStatus", "A"); // Active by default
            parameters.Add("@Usr_PhoneNo", "");
            parameters.Add("@Usr_OfficePhone", "");
            parameters.Add("@Usr_OffPhExtn", "");
            parameters.Add("@Usr_Designation", 0);
            parameters.Add("@Usr_CompanyID", 0);
            parameters.Add("@Usr_OrgnID", 0);
            parameters.Add("@Usr_GrpOrUserLvlPerm", dto.PermissionId);

            // Modules (default 0)
            parameters.Add("@Usr_MasterModule", 0);
            parameters.Add("@Usr_AuditModule", 0);
            parameters.Add("@Usr_RiskModule", 0);
            parameters.Add("@Usr_ComplianceModule", 0);
            parameters.Add("@Usr_BCMModule", 0);
            parameters.Add("@Usr_DigitalOfficeModule", 0);

            // Roles (default 0)
            parameters.Add("@Usr_MasterRole", 0);
            parameters.Add("@Usr_AuditRole", 0);
            parameters.Add("@Usr_RiskRole", 0);
            parameters.Add("@Usr_ComplianceRole", 0);
            parameters.Add("@Usr_BCMRole", 0);
            parameters.Add("@Usr_DigitalOfficeRole", 0);

            // Audit Info (default/hardcoded)
            if (dto.UserId == null || dto.UserId == 0) // Insert
            {
                parameters.Add("@Usr_CreatedBy", dto.CreatedBy);
                parameters.Add("@Usr_UpdatedBy", 0);
            }
            else // Update
            {
                parameters.Add("@Usr_CreatedBy", 0);
                parameters.Add("@Usr_UpdatedBy", dto.CreatedBy);
            }
            parameters.Add("@Usr_DelFlag", "N");
            parameters.Add("@Usr_IPAddress", "127.0.0.1"); // or from HttpContext
            parameters.Add("@Usr_CompId", 1);
            parameters.Add("@Usr_Type", "C");

            parameters.Add("@usr_IsSuperuser", 0);
            parameters.Add("@USR_DeptID", 0);
            parameters.Add("@USR_MemberType", 0);
            parameters.Add("@USR_Levelcode", 0);


            // Output params
            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Call SP
            await connection.ExecuteAsync("spEmployeeMaster", parameters, commandType: CommandType.StoredProcedure);

            int resultType = parameters.Get<int>("@iUpdateOrSave");
            int userId = parameters.Get<int>("@iOper");

            return resultType == 2
        ? "Employee updated successfully"
        : "Employee created successfully";

        }


        private string EncryptPassword(string plainText)
        {
            string encryptionKey = "ML736@mmcs";
            byte[] salt = new byte[]
            {
        0x49, 0x76, 0x61, 0x6E, 0x20,
        0x4D, 0x65, 0x64, 0x76, 0x65,
        0x64, 0x65, 0x76
            };

            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

            using var aes = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, salt);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.Close();
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public async Task<EmployeeInfo> GetEmployeeInfoByIdAsync(int userId, int companyId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // Step 2: Query to get employee info
            string query = @"
SELECT
    u.usr_Id AS UserId,
    u.usr_Code AS EmpCode,
    u.usr_FullName AS EmployeeName,
    u.usr_LoginName AS LoginName,
  
    u.usr_Email AS Email,
    u.usr_MobileNo AS MobileNo,
    u.Usr_Role AS RoleId,
    g.Mas_Description AS RoleName
FROM Sad_UserDetails u
LEFT JOIN SAD_GrpOrLvl_General_Master g ON u.Usr_Role = g.Mas_ID
WHERE u.usr_ID = @UserId
  AND u.Usr_CompId = @CompanyId";

            // Step 3: Execute query
            var employee = await connection.QueryFirstOrDefaultAsync<EmployeeInfo>(
                query,
                new { UserId = userId, CompanyId = companyId });

            return employee;
        }



    }


}

