using System.Text.Json.Serialization;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.ProfileSetting;
using static TracePca.Dto.ProfileSetting.ProfileSettingDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;
using System.Security.Cryptography;
using System.Text;

namespace TracePca.Service.ProfileSetting
{
    public class ProfileSettingService : ProfileSettingInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileSettingService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
           _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetUserProfile
        public async Task<IEnumerable<TracePaGetUserProfileDto>> GetUserProfileAsync(int iUserId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"SELECT 
        u.usr_Id AS UserId,
        u.usr_MobileNo AS MobileNo,
        u.usr_Email AS Email,
        u.Usr_Experience AS Experience,
        u.usr_LoginName AS LoginName,
        u.usr_Code AS SAPCode,
        u.usr_FullName AS EmpName,
        d.Mas_Description AS Designation,        
        r.Mas_Description AS Role,               
        CASE 
            WHEN u.usr_PermAddId = 0 THEN 'Role based'
            ELSE 'User based'
        END AS Permission       
    FROM sad_userDetails u
    LEFT JOIN SAD_GRPDESGN_General_Master d 
        ON u.usr_Designation = d.Mas_ID   
    LEFT JOIN SAD_GrpOrLvl_General_Master r 
        ON u.Usr_Role = r.Mas_ID          
    WHERE u.usr_Id = @UserId;";

            return await connection.QueryAsync<TracePaGetUserProfileDto>(query, new { UserId = iUserId });
        }

        //ChangePassword
        //public async Task<IEnumerable<TracePaChangePasswordDto>> PutChangePasswordAsync(TracePaChangePasswordDto dto)
        //{ 
        //    // ✅ Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // ✅ Step 2: Get the connection string
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    // ✅ Step 3: Use SqlConnection
        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var query = @"
        //UPDATE Sad_Userdetails 
        //SET 
        //    Usr_UpdatedBy = @UserId,
        //    Usr_UpdatedOn = GETDATE(),
        //    usr_PassWord = @Password
        //WHERE
        //    usr_LoginName = @LoginName 
        //    AND usr_Id = @UserId";

        //    var parameters = new
        //    {
        //        UserId = dto.UserId,
        //        Password = dto.NewPassword,
        //        LoginName = dto.LoginName
        //    };

        //    var rowsAffected = await connection.ExecuteAsync(query, parameters);

        //    if (rowsAffected > 0)
        //    {
        //        // Set status manually for success case
        //        dto.Status = "Success";
        //        return new List<TracePaChangePasswordDto> { dto };
        //    }

        //    // Optionally set status to something else
        //    dto.Status = "Failed";
        //    return new List<TracePaChangePasswordDto>();
        //}
        public async Task<IEnumerable<TracePaChangePasswordDto>> PutChangePasswordAsync(TracePaChangePasswordDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Optional — fetch existing encrypted password (for audit/log or validation if needed)
            var selectQuery = @"
SELECT usr_PassWord 
FROM Sad_Userdetails 
WHERE usr_LoginName = @LoginName AND usr_Id = @UserId";

            var existingEncryptedPassword = await connection.QueryFirstOrDefaultAsync<string>(selectQuery, new
            {
                dto.UserId,
                dto.LoginName
            });

            if (string.IsNullOrEmpty(existingEncryptedPassword))
                throw new Exception("User not found.");

            // ✅ Step 4: Encrypt the new password
            var newEncryptedPassword = EncryptPassword(dto.NewPassword);

            // ✅ Step 5: Update directly
            var updateQuery = @"
UPDATE Sad_Userdetails 
SET usr_PassWord = @NewPassword,
    Usr_UpdatedBy = @UserId,
    Usr_UpdatedOn = GETDATE()
WHERE usr_LoginName = @LoginName AND usr_Id = @UserId";

            var rowsAffected = await connection.ExecuteAsync(updateQuery, new
            {
                dto.UserId,
                dto.LoginName,
                NewPassword = newEncryptedPassword
            });

            if (rowsAffected == 0)
                throw new Exception("Password update failed.");

            return new List<TracePaChangePasswordDto> { dto };
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
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plainBytes, 0, plainBytes.Length);
            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        private string DecryptPassword(string encryptedBase64)
        {
            string decryptionKey = "ML736@mmcs";
            byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);

            byte[] salt = new byte[]
            {
        0x49, 0x76, 0x61, 0x6E, 0x20,
        0x4D, 0x65, 0x64, 0x76, 0x65,
        0x64, 0x65, 0x76
            };

            using var aes = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(decryptionKey, salt);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();

            return Encoding.Unicode.GetString(ms.ToArray());
        }


        //GetLicenseInformation
        public async Task<IEnumerable<TracePaLicenseInformationDto>> GetLicenseInformationAsync(string sEmailId, string sCustomerCode)
        {

            using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));

            var query = @"SELECT 
            MCR_ID as CustomerId,
            MCR_ProductKey,
            MCR_MP_ID as ModuleId,
            MCR_CustomerName as CustomerName,
            MCR_CustomerCode as CustomerCode,
            MCR_FromDate as FromDate,
            MCR_ToDate as ToDate,
            MCR_BillingFrequency as  BillingFrequency,
            MCR_DataSize as DataSize,
            MCR_NumberOfCustomers as NoOfCustomers,
            MCR_NumberOfUsers as NoOfUsers
            FROM MMCS_CustomerRegistration WHERE ',' + REPLACE(MCR_Emails, ' ', '') + ',' LIKE '%,' + @EmailId + ',%'
  AND MCR_CustomerCode = @CustomerCode";

            await connection.OpenAsync();

            return await connection.QueryAsync<TracePaLicenseInformationDto>(query, new { EmailId = sEmailId, CustomerCode = @sCustomerCode });
        }

        //UpdateUserProfile
        public async Task<int> UpdateUserProfileAsync(UpdateUserProfileDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        UPDATE sad_userDetails 
        SET Usr_Experience = @Experience
        WHERE usr_Id = @Id";

            await connection.ExecuteAsync(query, new
            {         
                Experience = dto.Experience,
                Id= dto.Id
            });

            return dto.Id;
        }
    }
}
