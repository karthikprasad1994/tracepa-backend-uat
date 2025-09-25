using System.Data;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using TracePca.Dto.Audit;
using TracePca.Dto.EmployeeMaster;
using TracePca.Interface.EmployeeMaster;

namespace TracePca.Service.EmployeeMaster
{
    public class EmployeeMaster : EmployeeMasterInterface
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
    u.usr_MobileNo AS MobileNo,
    u.Usr_GrpOrUserLvlPerm AS  PermissionId,
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


        public async Task<StatusDto> SaveEmployeeBasicDetailsAsync(EmployeeBasicDetailsDto dto)
        {
            var result = new StatusDto();

            // 1️⃣ Confirm Password check
            if (dto.Password != dto.ConfirmPassword)
            {
                result.StatusCode = 400;
                result.Message = "Password and Confirm Password do not match.";
                return result;
            }

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // 2️⃣ Duplicate check for Insert and Update
            if (!string.IsNullOrEmpty(dto.EmpCode) || !string.IsNullOrEmpty(dto.Email) || !string.IsNullOrEmpty(dto.MobileNo))
            {
                var duplicateQuery = @"
SELECT 
    STUFF(
        CASE WHEN EXISTS (SELECT 1 FROM Sad_UserDetails WHERE Usr_Code = @EmpCode AND (@UserId IS NULL OR Usr_ID <> @UserId)) THEN ',Employee Code' ELSE '' END +
        CASE WHEN EXISTS (SELECT 1 FROM Sad_UserDetails WHERE Usr_Email = @Email AND (@UserId IS NULL OR Usr_ID <> @UserId)) THEN ',Email' ELSE '' END +
        CASE WHEN EXISTS (SELECT 1 FROM Sad_UserDetails WHERE Usr_MobileNo = @MobileNo AND (@UserId IS NULL OR Usr_ID <> @UserId)) THEN ',Mobile No' ELSE '' END
    , 1, 1, '') AS DuplicateFields
";

                var duplicateFields = await connection.QueryFirstOrDefaultAsync<string>(duplicateQuery, new
                {
                    EmpCode = dto.EmpCode,
                    Email = dto.Email,
                    MobileNo = dto.MobileNo,
                    UserId = dto.UserId
                });

                if (!string.IsNullOrEmpty(duplicateFields))
                {
                    var fields = duplicateFields.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    if (fields.Length == 1)
                        result.Message = $"{fields[0]} already exists.";
                    else if (fields.Length == 2)
                        result.Message = $"{fields[0]} and {fields[1]} already exist.";
                    else
                        result.Message = string.Join(", ", fields.Take(fields.Length - 1)) + ", and " + fields.Last() + " already exist.";

                    result.StatusCode = 400;
                    return result;
                }
            }

            // 3️⃣ Prepare parameters for SP
            var parameters = new DynamicParameters();
            parameters.Add("@Usr_ID", dto.UserId ?? 0);
            parameters.Add("@Usr_Status", dto.UserId == 0 ? "U" : "C");
            parameters.Add("@Usr_FullName", dto.EmployeeName);
            parameters.Add("@Usr_LoginName", dto.LoginName);
            parameters.Add("@Usr_Password", EncryptPassword(dto.Password));
            parameters.Add("@Usr_Email", dto.Email);
            parameters.Add("@Usr_MobileNo", dto.MobileNo);
            parameters.Add("@Usr_Role", dto.RoleId);

            // ... add remaining SP parameters as before

            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // 4️⃣ Execute SP
            await connection.ExecuteAsync("spEmployeeMaster", parameters, commandType: CommandType.StoredProcedure);

            int resultType = parameters.Get<int>("@iUpdateOrSave");

            // 5️⃣ Update global registration DB
            using (var regConnection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection")))
            {
                string updateEmailSql = @"
UPDATE MMCS_CustomerRegistration
SET MCR_emails = 
    CASE 
        WHEN MCR_emails IS NULL OR MCR_emails = '' THEN @Email + ',' 
        WHEN CHARINDEX(',' + @Email + ',', ',' + MCR_emails + ',') > 0 THEN MCR_emails
        ELSE MCR_emails + @Email + ','
    END
WHERE MCR_CustomerCode = @CustomerCode";

                await regConnection.ExecuteAsync(updateEmailSql, new
                {
                    Email = dto.Email.Trim(),
                    CustomerCode = dbName
                });
            }

            result.StatusCode = 200;
            result.Message = resultType == 2 ? "Employee updated successfully" : "Employee created successfully";
            return result;
        }





        //        public async Task<string> SaveEmployeeBasicDetailsAsync(EmployeeBasicDetailsDto dto)
        //        {
        //            // 1️⃣ Confirm Password check
        //            if (dto.Password != dto.ConfirmPassword)
        //            {
        //                return "Password and Confirm Password do not match.";
        //            }

        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

        //            // 2️⃣ Duplicate check only while inserting
        //            if (dto.UserId == null || dto.UserId == 0)
        //            {
        //                var duplicateQuery = @"
        //SELECT 
        //    STUFF(
        //        CASE WHEN EXISTS (SELECT 1 FROM Sad_UserDetails WHERE Usr_Code = @EmpCode) THEN ',Employee Code' ELSE '' END +
        //        CASE WHEN EXISTS (SELECT 1 FROM Sad_UserDetails WHERE Usr_Email = @Email) THEN ',Email' ELSE '' END +
        //        CASE WHEN EXISTS (SELECT 1 FROM Sad_UserDetails WHERE Usr_MobileNo = @MobileNo) THEN ',Mobile No' ELSE '' END
        //    , 1, 1, '') AS DuplicateFields
        //";

        //                var duplicateFields = await connection.QueryFirstOrDefaultAsync<string>(duplicateQuery, new
        //                {
        //                    EmpCode = dto.EmpCode,
        //                    Email = dto.Email,
        //                    MobileNo = dto.MobileNo
        //                });

        //                if (!string.IsNullOrEmpty(duplicateFields))
        //                {
        //                    var fields = duplicateFields.Split(',', StringSplitOptions.RemoveEmptyEntries);

        //                    string message;
        //                    if (fields.Length == 1)
        //                        message = $"{fields[0]} already exists.";
        //                    else if (fields.Length == 2)
        //                        message = $"{fields[0]} and {fields[1]} already exists.";
        //                    else
        //                        message = string.Join(", ", fields.Take(fields.Length - 1)) + ", and " + fields.Last() + " already exists.";

        //                    return message;
        //                }
        //            }

        //            // 3️⃣ Prepare parameters for SP
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@Usr_ID", dto.UserId ?? 0);
        //            parameters.Add("@Usr_Status", dto.UserId == 0 ? "U" : "C");

        //            parameters.Add("@Usr_FullName", dto.EmployeeName);
        //            parameters.Add("@Usr_LoginName", dto.LoginName);
        //            parameters.Add("@Usr_Password", EncryptPassword(dto.Password));
        //            parameters.Add("@Usr_Email", dto.Email);
        //            parameters.Add("@Usr_MobileNo", dto.MobileNo);
        //            parameters.Add("@Usr_Role", dto.RoleId);

        //            // Extra params
        //            parameters.Add("@Usr_Node", 0);
        //            parameters.Add("@Usr_Code", dto.EmpCode);
        //            parameters.Add("@Usr_Category", 0);
        //            parameters.Add("@Usr_Suggetions", 0);
        //            parameters.Add("@usr_partner", 0);
        //            parameters.Add("@Usr_LevelGrp", 0);
        //            parameters.Add("@Usr_DutyStatus", "A");
        //            parameters.Add("@Usr_PhoneNo", "");
        //            parameters.Add("@Usr_OfficePhone", "");
        //            parameters.Add("@Usr_OffPhExtn", "");
        //            parameters.Add("@Usr_Designation", 0);
        //            parameters.Add("@Usr_CompanyID", 0);
        //            parameters.Add("@Usr_OrgnID", 0);
        //            parameters.Add("@Usr_GrpOrUserLvlPerm", dto.PermissionId);

        //            // Modules (default 0)
        //            parameters.Add("@Usr_MasterModule", 0);
        //            parameters.Add("@Usr_AuditModule", 0);
        //            parameters.Add("@Usr_RiskModule", 0);
        //            parameters.Add("@Usr_ComplianceModule", 0);
        //            parameters.Add("@Usr_BCMModule", 0);
        //            parameters.Add("@Usr_DigitalOfficeModule", 0);

        //            // Roles (default 0)
        //            parameters.Add("@Usr_MasterRole", 0);
        //            parameters.Add("@Usr_AuditRole", 0);
        //            parameters.Add("@Usr_RiskRole", 0);
        //            parameters.Add("@Usr_ComplianceRole", 0);
        //            parameters.Add("@Usr_BCMRole", 0);
        //            parameters.Add("@Usr_DigitalOfficeRole", 0);

        //            // Audit info
        //            if (dto.UserId == null || dto.UserId == 0)
        //            {
        //                parameters.Add("@Usr_CreatedBy", dto.CreatedBy);
        //                parameters.Add("@Usr_UpdatedBy", 0);
        //            }
        //            else
        //            {
        //                parameters.Add("@Usr_CreatedBy", 0);
        //                parameters.Add("@Usr_UpdatedBy", dto.CreatedBy);
        //            }

        //            parameters.Add("@Usr_DelFlag", "N");
        //            parameters.Add("@Usr_IPAddress", "127.0.0.1");
        //            parameters.Add("@Usr_CompId", 1);
        //            parameters.Add("@Usr_Type", "C");

        //            parameters.Add("@usr_IsSuperuser", 0);
        //            parameters.Add("@USR_DeptID", 0);
        //            parameters.Add("@USR_MemberType", 0);
        //            parameters.Add("@USR_Levelcode", 0);

        //            // Output params
        //            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //            // 4️⃣ Execute SP
        //            await connection.ExecuteAsync("spEmployeeMaster", parameters, commandType: CommandType.StoredProcedure);

        //            int resultType = parameters.Get<int>("@iUpdateOrSave");

        //            // 5️⃣ Update global registration DB (dynamic CustomerCode)
        //            using (var regConnection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection")))
        //            {
        //                string updateEmailSql = @"
        //UPDATE MMCS_CustomerRegistration
        //SET MCR_emails = 
        //    CASE 
        //        WHEN MCR_emails IS NULL OR MCR_emails = '' THEN @Email + ','
        //        WHEN CHARINDEX(',' + @Email + ',', ',' + MCR_emails + ',') > 0 THEN MCR_emails -- already exists
        //        ELSE MCR_emails + @Email + ','
        //    END
        //WHERE MCR_CustomerCode = @CustomerCode";

        //                await regConnection.ExecuteAsync(updateEmailSql, new
        //                {
        //                    Email = dto.Email.Trim(), // remove extra spaces
        //                    CustomerCode = dbName
        //                });


        //                return resultType == 2
        //                ? "Employee updated successfully"
        //                : "Employee created successfully";
        //            }

        //        }

        //public async Task<string> SaveEmployeeBasicDetailsAsync(EmployeeBasicDetailsDto dto)
        //{
        //    // Confirm Password check
        //    if (dto.Password != dto.ConfirmPassword)
        //    {
        //        return "Password and Confirm Password do not match.";
        //    }

        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

        //    // 🔹 Check duplicates only for Insert (UserId == 0)
        //    if (dto.UserId == null || dto.UserId == 0)
        //    {
        //        var duplicateQuery = @"
        //    SELECT TOP 1 
        //        CASE 
        //            WHEN Usr_Code = @EmpCode THEN 'Employee Code already exists.'
        //            WHEN Usr_Email = @Email THEN 'Email already exists.'
        //            WHEN Usr_FullName = @EmployeeName THEN 'Employee Name already exists.'
        //            WHEN Usr_MobileNo = @MobileNo THEN 'Mobile No already exists.'
        //            WHEN Usr_LoginName = @LoginName THEN 'Login Name already exists.'
        //        END
        //    FROM EmployeeMaster 
        //    WHERE Usr_Code = @EmpCode
        //       OR Usr_Email = @Email
        //       OR Usr_FullName = @EmployeeName
        //       OR Usr_MobileNo = @MobileNo
        //       OR Usr_LoginName = @LoginName";

        //        var duplicate = await connection.QueryFirstOrDefaultAsync<string>(duplicateQuery, new
        //        {
        //            EmpCode = dto.EmpCode,
        //            Email = dto.Email,
        //            EmployeeName = dto.EmployeeName,
        //            MobileNo = dto.MobileNo,
        //            LoginName = dto.LoginName
        //        });

        //        if (!string.IsNullOrEmpty(duplicate))
        //        {
        //            return duplicate; // return the specific error message
        //        }
        //    }

        //    var parameters = new DynamicParameters();

        //    // Insert (new user, UserId = 0) or Update (existing user, UserId > 0)
        //    parameters.Add("@Usr_ID", dto.UserId ?? 0);
        //    parameters.Add("@Usr_Status", dto.UserId == 0 ? "U" : "C");

        //    // Required fields
        //    parameters.Add("@Usr_FullName", dto.EmployeeName);
        //    parameters.Add("@Usr_LoginName", dto.LoginName);
        //    parameters.Add("@Usr_Password", EncryptPassword(dto.Password));
        //    parameters.Add("@Usr_Email", dto.Email);
        //    parameters.Add("@Usr_MobileNo", dto.MobileNo);
        //    parameters.Add("@Usr_Role", dto.RoleId);

        //    // Additional params
        //    parameters.Add("@Usr_Node", 0);
        //    parameters.Add("@Usr_Code", dto.EmpCode);
        //    parameters.Add("@Usr_Category", 0);
        //    parameters.Add("@Usr_Suggetions", 0);
        //    parameters.Add("@usr_partner", 0);
        //    parameters.Add("@Usr_LevelGrp", 0);
        //    parameters.Add("@Usr_DutyStatus", "A");
        //    parameters.Add("@Usr_PhoneNo", "");
        //    parameters.Add("@Usr_OfficePhone", "");
        //    parameters.Add("@Usr_OffPhExtn", "");
        //    parameters.Add("@Usr_Designation", 0);
        //    parameters.Add("@Usr_CompanyID", 0);
        //    parameters.Add("@Usr_OrgnID", 0);
        //    parameters.Add("@Usr_GrpOrUserLvlPerm", dto.PermissionId);

        //    // Modules (default 0)
        //    parameters.Add("@Usr_MasterModule", 0);
        //    parameters.Add("@Usr_AuditModule", 0);
        //    parameters.Add("@Usr_RiskModule", 0);
        //    parameters.Add("@Usr_ComplianceModule", 0);
        //    parameters.Add("@Usr_BCMModule", 0);
        //    parameters.Add("@Usr_DigitalOfficeModule", 0);

        //    // Roles (default 0)
        //    parameters.Add("@Usr_MasterRole", 0);
        //    parameters.Add("@Usr_AuditRole", 0);
        //    parameters.Add("@Usr_RiskRole", 0);
        //    parameters.Add("@Usr_ComplianceRole", 0);
        //    parameters.Add("@Usr_BCMRole", 0);
        //    parameters.Add("@Usr_DigitalOfficeRole", 0);

        //    // Audit Info
        //    if (dto.UserId == null || dto.UserId == 0)
        //    {
        //        parameters.Add("@Usr_CreatedBy", dto.CreatedBy);
        //        parameters.Add("@Usr_UpdatedBy", 0);
        //    }
        //    else
        //    {
        //        parameters.Add("@Usr_CreatedBy", 0);
        //        parameters.Add("@Usr_UpdatedBy", dto.CreatedBy);
        //    }

        //    parameters.Add("@Usr_DelFlag", "N");
        //    parameters.Add("@Usr_IPAddress", "127.0.0.1");
        //    parameters.Add("@Usr_CompId", 1);
        //    parameters.Add("@Usr_Type", "C");

        //    parameters.Add("@usr_IsSuperuser", 0);
        //    parameters.Add("@USR_DeptID", 0);
        //    parameters.Add("@USR_MemberType", 0);
        //    parameters.Add("@USR_Levelcode", 0);

        //    // Output params
        //    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //    // Call SP
        //    await connection.ExecuteAsync("spEmployeeMaster", parameters, commandType: CommandType.StoredProcedure);

        //    int resultType = parameters.Get<int>("@iUpdateOrSave");

        //    return resultType == 2
        //        ? "Employee updated successfully"
        //        : "Employee created successfully";
        //}


        //public async Task<string> SaveEmployeeBasicDetailsAsync(EmployeeBasicDetailsDto dto)
        //{
        //    // Confirm Password check
        //    if (dto.Password != dto.ConfirmPassword)
        //    {
        //        return "Password and Confirm Password do not match.";
        //    }

        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

        //    var parameters = new DynamicParameters();

        //    // Insert (new user, UserId = 0) or Update (existing user, UserId > 0)
        //    parameters.Add("@Usr_ID", dto.UserId ?? 0);
        //    parameters.Add("@Usr_Status", dto.UserId == 0 ? "U" : "C");


        //    // Required fields (mapped from DTO)
        //    parameters.Add("@Usr_FullName", dto.EmployeeName);
        //    parameters.Add("@Usr_LoginName", dto.LoginName);
        //    parameters.Add("@Usr_Password", EncryptPassword(dto.Password));
        //    parameters.Add("@Usr_Email", dto.Email);
        //    parameters.Add("@Usr_MobileNo", dto.MobileNo);
        //    parameters.Add("@Usr_Role", dto.RoleId);

        //    // Additional SP params (set from DTO if present, else default values)
        //    // Additional required parameters (hardcoded/default values)
        //    parameters.Add("@Usr_Node", 0);
        //    parameters.Add("@Usr_Code", dto.EmpCode);
        //    parameters.Add("@Usr_Category", 0);
        //    parameters.Add("@Usr_Suggetions", 0);
        //    parameters.Add("@usr_partner", 0);
        //    parameters.Add("@Usr_LevelGrp", 0);
        //    parameters.Add("@Usr_DutyStatus", "A"); // Active by default
        //    parameters.Add("@Usr_PhoneNo", "");
        //    parameters.Add("@Usr_OfficePhone", "");
        //    parameters.Add("@Usr_OffPhExtn", "");
        //    parameters.Add("@Usr_Designation", 0);
        //    parameters.Add("@Usr_CompanyID", 0);
        //    parameters.Add("@Usr_OrgnID", 0);
        //    parameters.Add("@Usr_GrpOrUserLvlPerm", dto.PermissionId);

        //    // Modules (default 0)
        //    parameters.Add("@Usr_MasterModule", 0);
        //    parameters.Add("@Usr_AuditModule", 0);
        //    parameters.Add("@Usr_RiskModule", 0);
        //    parameters.Add("@Usr_ComplianceModule", 0);
        //    parameters.Add("@Usr_BCMModule", 0);
        //    parameters.Add("@Usr_DigitalOfficeModule", 0);

        //    // Roles (default 0)
        //    parameters.Add("@Usr_MasterRole", 0);
        //    parameters.Add("@Usr_AuditRole", 0);
        //    parameters.Add("@Usr_RiskRole", 0);
        //    parameters.Add("@Usr_ComplianceRole", 0);
        //    parameters.Add("@Usr_BCMRole", 0);
        //    parameters.Add("@Usr_DigitalOfficeRole", 0);

        //    // Audit Info (default/hardcoded)
        //    if (dto.UserId == null || dto.UserId == 0) // Insert
        //    {
        //        parameters.Add("@Usr_CreatedBy", dto.CreatedBy);
        //        parameters.Add("@Usr_UpdatedBy", 0);
        //    }
        //    else // Update
        //    {
        //        parameters.Add("@Usr_CreatedBy", 0);
        //        parameters.Add("@Usr_UpdatedBy", dto.CreatedBy);
        //    }
        //    parameters.Add("@Usr_DelFlag", "N");
        //    parameters.Add("@Usr_IPAddress", "127.0.0.1"); // or from HttpContext
        //    parameters.Add("@Usr_CompId", 1);
        //    parameters.Add("@Usr_Type", "C");

        //    parameters.Add("@usr_IsSuperuser", 0);
        //    parameters.Add("@USR_DeptID", 0);
        //    parameters.Add("@USR_MemberType", 0);
        //    parameters.Add("@USR_Levelcode", 0);


        //    // Output params
        //    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //    // Call SP
        //    await connection.ExecuteAsync("spEmployeeMaster", parameters, commandType: CommandType.StoredProcedure);

        //    int resultType = parameters.Get<int>("@iUpdateOrSave");
        //    int userId = parameters.Get<int>("@iOper");

        //    return resultType == 2
        //? "Employee updated successfully"
        //: "Employee created successfully";

        //}


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

        public async Task<(bool IsSuccess, string Message)> ToggleUserStatusAsync(int employeeId)
        {
            try
            {
                // ✅ Step 1: Get DB name from session
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    return (false, "CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Open connection using dynamic DB
                await using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
                await connection.OpenAsync();

                // ✅ Step 3: Update query
                const string query = @"
UPDATE Sad_UserDetails
SET Usr_DutyStatus = 
    CASE 
        WHEN Usr_DutyStatus = 'A' THEN 'D'
        WHEN Usr_DutyStatus = 'D' THEN 'A'
        ELSE Usr_DutyStatus
    END
WHERE usr_Id = @EmployeeId";

                var rowsAffected = await connection.ExecuteAsync(query, new { EmployeeId = employeeId });

                if (rowsAffected > 0)
                    return (true, "User status updated successfully");

                return (false, "User not found");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating user status: {ex.Message}");
            }
        }

            //public async Task<string> SaveEmployeeBasicDetailsAsync(EmployeeBasicDetailsDto dto)
            //{
            //    // Confirm Password check
            //    if (dto.Password != dto.ConfirmPassword)
            //    {
            //        return "Password and Confirm Password do not match.";
            //    }

            //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            //    if (string.IsNullOrEmpty(dbName))
            //        throw new Exception("CustomerCode is missing in session. Please log in again.");

            //    using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            //    var parameters = new DynamicParameters();

            //    // Insert (new user, UserId = 0) or Update (existing user, UserId > 0)
            //    parameters.Add("@Usr_ID", dto.UserId ?? 0);
            //    parameters.Add("@Usr_Status", dto.UserId == 0 ? "U" : "C");


            //    // Required fields (mapped from DTO)
            //    parameters.Add("@Usr_FullName", dto.EmployeeName);
            //    parameters.Add("@Usr_LoginName", dto.LoginName);
            //    parameters.Add("@Usr_Password", EncryptPassword(dto.Password));
            //    parameters.Add("@Usr_Email", dto.Email);
            //    parameters.Add("@Usr_MobileNo", dto.MobileNo);
            //    parameters.Add("@Usr_Role", dto.RoleId);

            //    // Additional SP params (set from DTO if present, else default values)
            //    // Additional required parameters (hardcoded/default values)
            //    parameters.Add("@Usr_Node", 0);
            //    parameters.Add("@Usr_Code", dto.EmpCode);
            //    parameters.Add("@Usr_Category", 0);
            //    parameters.Add("@Usr_Suggetions", 0);
            //    parameters.Add("@usr_partner", 0);
            //    parameters.Add("@Usr_LevelGrp", 0);
            //    parameters.Add("@Usr_DutyStatus", "A"); // Active by default
            //    parameters.Add("@Usr_PhoneNo", "");
            //    parameters.Add("@Usr_OfficePhone", "");
            //    parameters.Add("@Usr_OffPhExtn", "");
            //    parameters.Add("@Usr_Designation", 0);
            //    parameters.Add("@Usr_CompanyID", 0);
            //    parameters.Add("@Usr_OrgnID", 0);
            //    parameters.Add("@Usr_GrpOrUserLvlPerm", dto.PermissionId);

            //    // Modules (default 0)
            //    parameters.Add("@Usr_MasterModule", 0);
            //    parameters.Add("@Usr_AuditModule", 0);
            //    parameters.Add("@Usr_RiskModule", 0);
            //    parameters.Add("@Usr_ComplianceModule", 0);
            //    parameters.Add("@Usr_BCMModule", 0);
            //    parameters.Add("@Usr_DigitalOfficeModule", 0);

            //    // Roles (default 0)
            //    parameters.Add("@Usr_MasterRole", 0);
            //    parameters.Add("@Usr_AuditRole", 0);
            //    parameters.Add("@Usr_RiskRole", 0);
            //    parameters.Add("@Usr_ComplianceRole", 0);
            //    parameters.Add("@Usr_BCMRole", 0);
            //    parameters.Add("@Usr_DigitalOfficeRole", 0);

            //    // Audit Info (default/hardcoded)
            //    if (dto.UserId == null || dto.UserId == 0) // Insert
            //    {
            //        parameters.Add("@Usr_CreatedBy", dto.CreatedBy);
            //        parameters.Add("@Usr_UpdatedBy", 0);
            //    }
            //    else // Update
            //    {
            //        parameters.Add("@Usr_CreatedBy", 0);
            //        parameters.Add("@Usr_UpdatedBy", dto.CreatedBy);
            //    }
            //    parameters.Add("@Usr_DelFlag", "N");
            //    parameters.Add("@Usr_IPAddress", "127.0.0.1"); // or from HttpContext
            //    parameters.Add("@Usr_CompId", 1);
            //    parameters.Add("@Usr_Type", "C");

            //    parameters.Add("@usr_IsSuperuser", 0);
            //    parameters.Add("@USR_DeptID", 0);
            //    parameters.Add("@USR_MemberType", 0);
            //    parameters.Add("@USR_Levelcode", 0);


            //    // Output params
            //    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            //    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            //    // Call SP
            //    await connection.ExecuteAsync("spEmployeeMaster", parameters, commandType: CommandType.StoredProcedure);

            //    int resultType = parameters.Get<int>("@iUpdateOrSave");
            //    int userId = parameters.Get<int>("@iOper");

            //    return resultType == 2
            //? "Employee updated successfully"
            //: "Employee created successfully";

            //}


            //       private string EncryptPassword(string plainText)
            //       {
            //           string encryptionKey = "ML736@mmcs";
            //           byte[] salt = new byte[]
            //           {
            //0x49, 0x76, 0x61, 0x6E, 0x20,
            //0x4D, 0x65, 0x64, 0x76, 0x65,
            //0x64, 0x65, 0x76
            //           };

            //           byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

            //           using var aes = Aes.Create();
            //           var pdb = new Rfc2898DeriveBytes(encryptionKey, salt);
            //           aes.Key = pdb.GetBytes(32);
            //           aes.IV = pdb.GetBytes(16);

            //           using var ms = new MemoryStream();
            //           using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            //           {
            //               cs.Write(plainBytes, 0, plainBytes.Length);
            //               cs.Close();
            //           }

            //           return Convert.ToBase64String(ms.ToArray());
            //       }



        }


}

