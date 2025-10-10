using System.Text.Json.Serialization;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.ProfileSetting;
using static TracePca.Dto.ProfileSetting.ProfileSettingDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

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
        public async Task<IEnumerable<TracePaChangePasswordDto>> PutChangePasswordAsync(TracePaChangePasswordDto dto)
        { 
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        UPDATE Sad_Userdetails 
        SET 
            Usr_UpdatedBy = @UserId,
            Usr_UpdatedOn = GETDATE(),
            usr_PassWord = @Password
        WHERE
            usr_LoginName = @LoginName 
            AND usr_Id = @UserId";

            var parameters = new
            {
                UserId = dto.UserId,
                Password = dto.NewPassword,
                LoginName = dto.LoginName
            };

            var rowsAffected = await connection.ExecuteAsync(query, parameters);

            if (rowsAffected > 0)
            {
                // Set status manually for success case
                dto.Status = "Success";
                return new List<TracePaChangePasswordDto> { dto };
            }

            // Optionally set status to something else
            dto.Status = "Failed";
            return new List<TracePaChangePasswordDto>();
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
