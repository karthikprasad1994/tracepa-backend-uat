using TracePca.Dto.EmployeeMaster;

namespace TracePca.Interface.EmployeeMaster
{
    public interface EmployeeMasterInterface
    {
        Task<IEnumerable<Dto.EmployeeMaster.EmployeesDto>> GetUserFullNameAsync();
        Task<IEnumerable<EmployeeDetailsDto>> GetEmployeeDetailsAsync(int companyId);


    }
}
