using static TracePca.Dto.SuperMaster.ExcelInformationDto;

using TracePca.Dto.SuperMaster;
using Microsoft.AspNetCore.Mvc;



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

        //DownloadEmployeeMaster
        EmployeeMasterResult GetEmployeeMasterExcelTemplate();

        //DownloadClientDetails
        ClientDetailsResult GetClientDetailsExcelTemplate();

        //DownloadClientuser
        ClientUserResult GetClientUserExcelTemplate();

        //DownloadExcelTemplateFiles
<<<<<<< HEAD
        ExcelTemplateResult GetExcelTemplate(string templateName);
=======
        ExcelInformationTemplateResult GetExcelTemplate(string FileName);

        Task<List<string>> UploadAuditTypeAndCheckpointsAsync(int compId, int userId, IFormFile file);

        Task<List<string>> UploadTaskAndSubTasksAsync(int compId, int userId, IFormFile file);
>>>>>>> 2ed7780949550b8113b9d4ee2a732733d8fcb143
    }
}
