using TracePca.Dto.CustomerUserMaster;

namespace TracePca.Interface.CustomerUserMaster
{
    public interface CustomerUserMasterInterface
    {
        Task<IEnumerable<CustomerUsersDetailsDto>> GetAllUserDetailsAsync(int companyId);
        Task<IEnumerable<UserDropdownDto>> LoadExistingUsersAsync(int companyId, string search = "");
        Task<IEnumerable<CustomerDropdownDto>> LoadActiveCustomersAsync(int companyId);
        Task<string> InsertCustomerUsersDetailsAsync(CreateCustomerUsersDto dto);
    }
}
