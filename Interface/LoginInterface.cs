using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Dto;
using TracePca.Models;
using TracePca.Models.CustomerRegistration;

namespace TracePca.Interface
{
    public interface LoginInterface
    {


        Task<object> GetAllUsersAsync();
        Task<IActionResult> SignUpUserAsync(RegistrationDto registerModel);
        Task<bool> VerifyOtpJwtAsync(string token, string enteredOtp);
      //  Task<(string Token, string Otp)> GenerateAndSendOtpJwtAsync(string email);
        Task<(bool Success, string Message, string? OtpToken)> GenerateAndSendOtpJwtAsync(string email);
        Task<bool> LogoutUserAsync(string accessToken);

      //  Task<LoginResponse> AuthenticateUserAsync(string email, string password);
      // Task<LoginResponse> LoginUser(string email, string password);
        Task<LoginResponse> LoginUserAsync(string email, string password);
        SqlConnection GetConnection(string customerCode);
        Task<(bool Success, string Message)> CheckAndAddAccessCodeConnectionStringAsync(string accessCode);

        //  Task<string> GetLoginUserPermissionTraceAsync(UserPermissionRequestDto dto);
       // Task<List<OperationPermissionDto>> GetLoginUserPermissionTraceAsync(UserPermissionRequestDto dto);
       // Task<string> GetLoginUserPermissionTraceAsync(int companyId, int userId, string moduleCode, int checkModuleId);
        Task<List<FormPermissionDto>> GetUserPermissionsWithFormNameAsync(int companyId, int userId);
    }
}
