using TracePca.Dto.CustomerMaster;

namespace TracePca.Interface
{
    public interface CustomerMasterInterface
    {
        Task<IEnumerable<CustomerDetailsDto>> GetCustomersWithStatusAsync(int companyId);

    }
}
