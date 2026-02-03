using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Dto;
using TracePca.Dto.Authentication;
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
        Task<IActionResult> SignUpUserViaGoogleAsync(GoogleAuthDto dto);
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
        Task<IEnumerable<LogInfoDto>> GetUserLoginLogsAsync();
        Task<IEnumerable<ModuleDto>> GetModulesByMpIdAsync(int mpId);
        Task UpdateCustomerModulesAsync(int customerId, List<int> moduleIds);
        Task<List<CustomerModuleDetailDto>> GetCustomerModulesAsync(int customerId);
        Task<bool> SendWelcomeEmailAsync(string gmail, string password);


        #region API For DashboardsDetails

        Task<int> GetTotalClientsAsync();

        Task<int> GetNewSignup30DaysAsync();

        Task<int> GetTrialUsersAsync();
        Task<int> GetPendingIssueAsync();

        Task<int> GetResolvedIssueAsync();

        Task<int> GetApprovalStatusAsync();

        Task<DashboardCounts> GetDashboardCardDetailsAsync();

        Task<IEnumerable<ClientDetails>> GetClientDetailsAsync();
        Task<(bool Success, string Message, string? OtpToken)> ForgPassSendOtpJwtAsync(string email);
        Task<(bool Success, string Message)> UpdatePasswordAsync(UpdatePasswordDto dto);

        Task<int> GetTodayLoginAsync(int CompID);

        Task<int> GetTodayLogoutAsync(int CompID);

        Task<int> GetTotalTimeSpentAsync(int CompID);

        Task<IEnumerable<ClientViewDetails>> GetClientFullDetailsAsync(int FirmID);

        Task<string> GetUserTrialOrPaidAsync(string Email);

        Task<string> GetTrialRemainingDaysAsync(string Email);
        #endregion

    }
}
