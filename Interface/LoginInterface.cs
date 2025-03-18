using TracePca.Models;
using TracePca.Models.CustomerRegistration;

namespace TracePca.Interface
{
    public interface LoginInterface
    {
        Task<object> GetAllUsersAsync();
        Task<object> AddUsersAsync(SadUserDetail User, MmcsCustomerRegistration Customer);
        Task<object> AuthenticateUserAsync(string email, string password);

    }

}
