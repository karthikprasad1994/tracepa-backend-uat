using static TracePca.Dto.SuperMaster.ExcelInformationDto;

using TracePca.Dto.SuperMaster;

namespace TracePca.Interface.SuperMaster
{
    public interface ExcelInformationInterfaces
    {
        //ValidateEmployeeMasters
        Task<object> ValidateExcelDataAsync(int CompId, List<SuperMasterValidateEmployeeDto> employees);

        //SaveEmployeeMaster
        Task<int[]> SuperMasterSaveEmployeeDetailsAsync(int CompId, SuperMasterSaveEmployeeMasterDto objEmp);

        //ValidateClientDetails
        Task<object> ValidateClientDetailsAsync(int CompId, List<SuperMasterValidateClientDetailsDto> employees);

        //SaveClientDetails
        Task<int[]> SuperMasterSaveCustomerDetailsAsync(int CompId, SuperMasterSaveClientDetailsDto objCust);

        //SaveClientUser
        Task<int[]> SuperMasterSaveClientUserAsync(int CompId, SaveClientUserDto objEmp);
    }
}
