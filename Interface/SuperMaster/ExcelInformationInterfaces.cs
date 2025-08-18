using static TracePca.Dto.SuperMaster.ExcelInformationDto;

using TracePca.Dto.SuperMaster;

namespace TracePca.Interface.SuperMaster
{
    public interface ExcelInformationInterfaces
    {
        //ValidateEmployeeMasters
        Task<List<string>> SaveEmployeeDetailsAsync(int compId, IFormFile file);

        //SaveEmployeeMaster
        Task<List<int[]>> SuperMasterSaveEmployeeDetailsAsync(int CompId, List<SuperMasterSaveEmployeeMasterDto> employees);

        //UploadClientDetails
        Task<List<int>> UploadClientDetailsAsync(int CompId, IFormFile excelFile, string sheetName);

        //SaveClientDetails
        Task<List<int[]>> SuperMasterSaveCustomerDetailsAsync(int CompId, List<SuperMasterSaveCustomerDto> customers);

        //SaveClientUser
        Task<List<int[]>> SuperMasterSaveClientUserAsync(int CompId, List<SaveClientUserDto> clientUser);
    }
}
