using static TracePca.Dto.SuperMaster.ExcelInformationDto;

using TracePca.Dto.SuperMaster;

namespace TracePca.Interface.SuperMaster
{
    public interface ExcelInformationInterfaces
    {
        //ValidateClientDetailsValidateClientDetails
        Task<IEnumerable<SuperMasterValidateClientDetailsResult>> SuperMasterValidateClientDetailsExcelAsync(SuperMasterValidateClientDetailsResult file);

        //SaveEmployeeMaster
        Task<int[]> SuperMasterSaveEmployeeDetailsAsync(int CompId, SuperMasterSaveEmployeeMasterDto objEmp);

        //SaveClientDetails
        Task<int[]> SuperMasterSaveCustomerDetailsAsync(int CompId, SuperMasterSaveClientDetailsDto objCust);

        //SaveClientUser
        Task<int[]> SaveClientUserAsync(int CompId, SuperMasterSaveClientUserDto objCust);
    }
}
