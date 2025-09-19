using TracePca.Dto.EmployeeMaster;

namespace TracePca.Interface.EmployeeMaster
{
    public interface EmployeeMasterInterface
    {
        Task<IEnumerable<Dto.EmployeeMaster.EmployeesDto>> GetUserFullNameAsync();
        Task<IEnumerable<EmployeeDetailsDto>> GetEmployeeDetailsAsync(int companyId);
        Task<IEnumerable<RolesDto>> GetRolesAsync();
        Task<string> SaveEmployeeBasicDetailsAsync(EmployeeBasicDetailsDto dto);

        Task<EmployeeInfo> GetEmployeeInfoByIdAsync(int userId, int companyId);
        Task<(bool IsSuccess, string Message)> ToggleUserStatusAsync(int EmployeeId);






    }
}
