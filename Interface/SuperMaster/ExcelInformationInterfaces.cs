using static TracePca.Dto.SuperMaster.ExcelInformationDto;

using TracePca.Dto.SuperMaster;
using Microsoft.AspNetCore.Mvc;



namespace TracePca.Interface.SuperMaster
{
    public interface ExcelInformationInterfaces
    {
        //SaveEmployeeMaster
        Task<List<int[]>> SuperMasterSaveEmployeeDetailsAsync(int CompId, List<SuperMasterSaveEmployeeMasterDto> employees);

        //SaveClientDetails
        Task<List<int[]>> SuperMasterSaveCustomerDetailsAsync(int CompId, List<SuperMasterSaveCustomerDto> customers);

        //SaveClientUser
        Task<List<int[]>> SuperMasterSaveClientUserAsync(int CompId, List<SaveClientUserDto> clientUser);

        //DownloadEmployeeMaster
        EmployeeMasterResult GetEmployeeMasterExcelTemplate();

        //DownloadClientDetails
        ClientDetailsResult GetClientDetailsExcelTemplate();

        //DownloadClientuser
        ClientUserResult GetClientUserExcelTemplate();

        //DownloadExcelTemplateFiles
        ExcelTemplateResult GetExcelTemplate(string templateName);

        //ExcelInformationTemplateResult GetExcelTemplate(string FileName);

        Task<List<string>> UploadAuditTypeAndCheckpointsAsync(int compId, int userId, IFormFile file);

        Task<List<string>> UploadTaskAndSubTasksAsync(int compId, int userId, IFormFile file);

        //ChangedUploadEmployeeMasters
        Task<List<string>> UploadEmployeeDetailsAsync(int compId, IFormFile file);

        //ChangedUploadClientUser
        Task<List<string>> UploadClientUsersAsync(int compId, IFormFile file);

        //ChangedUploadClientDetails
        Task<List<string>> UploadClientDetailsAsync(int compId, IFormFile file);
    }
}
