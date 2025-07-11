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

        Task<LoginResponse> AuthenticateUserAsync(string email, string password);
       // Task<LoginResponse> LoginUser(string email, string password);
        Task<LoginResponse> LoginUserAsync(string email, string password);
        SqlConnection GetConnection(string customerCode);
        Task<(bool Success, string Message)> CheckAndAddAccessCodeConnectionStringAsync(string accessCode);

          Task<string> GetLoginUserPermissionTraceAsync(UserPermissionRequestDto dto);
    }
}
