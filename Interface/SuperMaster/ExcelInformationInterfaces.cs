using static TracePca.Dto.SuperMaster.ExcelInformationDto;

using TracePca.Dto.SuperMaster;

namespace TracePca.Interface.SuperMaster
{
    public interface ExcelInformationInterfaces
    {
        //UploadEmployeeMasters
        Task<List<string>> UploadEmployeeDetailsAsync(int compId, IFormFile file);

        //SaveEmployeeMaster
        Task<List<int[]>> SuperMasterSaveEmployeeDetailsAsync(int CompId, List<SuperMasterSaveEmployeeMasterDto> employees);

        //UploadClientDetails
        Task<List<string>> UploadClientDetailsAsync(int compId, IFormFile file);

        //SaveClientDetails
        Task<List<int[]>> SuperMasterSaveCustomerDetailsAsync(int CompId, List<SuperMasterSaveCustomerDto> customers);

        //UploadClientUser
        Task<List<string>> UploadClientUserAsync(int compId, IFormFile file);

        //SaveClientUser
        Task<List<int[]>> SuperMasterSaveClientUserAsync(int CompId, List<SaveClientUserDto> clientUser);
    }
}
