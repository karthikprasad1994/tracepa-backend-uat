using TracePca.Dto.CustomerMaster;
using TracePca.Dto.EmployeeMaster;

namespace TracePca.Interface
{
    public interface CustomerMasterInterface
    {
        Task<IEnumerable<CustomerDetailsDto>> GetCustomersWithStatusAsync(int companyId);
        Task<IEnumerable<ServicesDto>> GetServicesAsync(int companyId);
        Task<IEnumerable<OrganizationDto>> GetOrganizationsAsync(int companyId);
        Task<IEnumerable<IndustryTypeDto>> GetIndustryTypesAsync(int companyId);
        Task<IEnumerable<ManagementTypeDto>> GetManagementTypesAsync(int companyId);
        Task<string> SaveCustomerMasterAsync(CreateCustomerMasterDto dto);
        Task<(bool IsSuccess, string Message)> ToggleCustomerStatusAsync(int CustId);

    }
}
