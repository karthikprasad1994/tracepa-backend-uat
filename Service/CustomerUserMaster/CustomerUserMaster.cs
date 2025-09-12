using System.Data;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Dto.CustomerUserMaster;
using TracePca.Dto.EmployeeMaster;
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


        public async Task<IEnumerable<UserDropdownDto>> LoadExistingUsersAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // ✅ Step 2: SQL query without search filter
            string query = @"
        SELECT 
            Usr_ID AS UserId,
            (Usr_FullName + ' - ' + Usr_Code) AS UserName
        FROM Sad_UserDetails
        WHERE Usr_CompID = @CompanyId
          AND Usr_Node = 0
          AND Usr_OrgnID = 0
        ORDER BY Usr_FullName";

            // ✅ Step 3: Execute with Dapper
            var users = await connection.QueryAsync<UserDropdownDto>(query, new
            {
                CompanyId = companyId
            });

            return users;
        }


        public async Task<IEnumerable<CustomerDropdownDto>> LoadActiveCustomersAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // ✅ Step 2: SQL Query
            string query = @"
        SELECT 
            Cust_Id AS CustId, 
            Cust_Name AS CustName
        FROM SAD_CUSTOMER_MASTER
        WHERE CUST_DelFlg = 'A' 
          AND Cust_CompId = @CompanyId
        ORDER BY Cust_Name";

            // ✅ Step 3: Execute with Dapper
            var customers = await connection.QueryAsync<CustomerDropdownDto>(query, new { CompanyId = companyId });

            return customers;
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
    a.Usr_Role AS RoleId,
    a.usr_Code AS EmpCode,
    a.Usr_GrpOrUserLvlPerm AS PermissionId,
    a.usr_FullName AS EmployeeName,
    a.Usr_LoginName AS LoginName,
    a.usr_Email AS Email,
    a.Usr_CompanyId As CustomerId,
    a.Usr_MobileNo AS MobileNo,
    a.Usr_OfficePhone AS OfficePhoneNo,
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
        public async Task<string> InsertCustomerUsersDetailsAsync(CreateCustomerUsersDto dto)
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
            parameters.Add("@Usr_FullName", dto.UserName);
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
            parameters.Add("@Usr_OfficePhone", dto.OfficePhoneNo);
            parameters.Add("@Usr_OffPhExtn", "");
            parameters.Add("@Usr_Designation", 0);
            parameters.Add("@Usr_CompanyID", dto.CustomerId);
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
        ? "customer updated successfully"
        : "Customer created successfully";

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



    }
}
