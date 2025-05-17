using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.ProfileSetting;
using static TracePca.Dto.ProfileSetting.ProfileSettingDto;

namespace TracePca.Service.ProfileSetting
{
    public class ProfileSettingService : ProfileSettingInterface
    {
        private readonly IConfiguration _configuration;
        public ProfileSettingService(IConfiguration configuration)
        {
           _configuration = configuration;
        }

        //GetUserProfile
        public async Task<IEnumerable<TracePaGetUserProfileDto>> GetUserProfileAsync(int iUserId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection1"));

            var query = @"SELECT 
            usr_Id as UserId,
            usr_MobileNo as MobileNo,
            usr_Email as Email,
            Usr_Experience as Experience,
            usr_LoginName as Loginname,
            usr_Code as SAPCode,
            usr_FullName as EmpName,
            usr_Designation as Designation,
            Usr_Role as Role,
            usr_PermAddId as Permission
            FROM sad_userDetails WHERE usr_Id = @UserId";

            await connection.OpenAsync();

            return await connection.QueryAsync<TracePaGetUserProfileDto>(query, new { UserId = iUserId });
        }

        //ChangePassword
        public async Task<IEnumerable<TracePaChangePasswordDto>> PutChangePasswordAsync(string LoginName, int UserId, TracePaChangePasswordDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection1"));

            var query = @"
        UPDATE Sad_Userdetails 
        SET 
            Usr_UpdatedBy = @UserId,
            Usr_UpdatedOn = GETDATE(),
            usr_PassWord = @Password
        WHERE
            usr_LoginName = @LoginName 
            AND usr_Id = @UserId";

            await connection.OpenAsync();

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
        public async Task<IEnumerable<TracePaLicenseInformationDto>> GetLicenseInformationAsync(int iCustomerId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection2"));

            var query = @"SELECT 
            MCR_ID as CustomerId,
            MCR_MP_ID as ModuleId,
            MCR_CustomerName as CustomerName,
            MCR_CustomerCode as CustomerCode,
            MCR_FromDate as FromDate,
            MCR_ToDate as ToDate,
            MCR_BillingFrequency as  BillingFrequency,
            MCR_DataSize as DataSize,
            MCR_NumberOfCustomers as NoOfCustomers,
            MCR_NumberOfUsers as NoOfUsers
            FROM MMCS_CustomerRegistration WHERE MCR_ID = @CustomerId";

            await connection.OpenAsync();

            return await connection.QueryAsync<TracePaLicenseInformationDto>(query, new { CustomerId = iCustomerId });
        }
    }
}
