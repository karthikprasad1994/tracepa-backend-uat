using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;
using TracePca.Models;
using TracePca.Models.CustomerRegistration;

namespace TracePca.Interface
{
    public interface LoginInterface
    {
        Task<object> GetAllUsersAsync();
        Task<IActionResult> SignUpUserAsync(RegistrationDto registerModel);



        Task<LoginResponse> AuthenticateUserAsync(string email, string password);

    }

}
