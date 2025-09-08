using TracePca.Dto.CustomerUserMaster;

namespace TracePca.Interface.CustomerUserMaster
{
    public interface CustomerUserMasterInterface
    {
        Task<IEnumerable<CustomerUsersDetailsDto>> GetAllUserDetailsAsync(int companyId);
    }
}
