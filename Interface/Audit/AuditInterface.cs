using Org.BouncyCastle.Tls;
using TracePca.Dto;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditInterface
    {
        //Task<CustomerAuditDropdownDto> GetCustomerAuditDropdownAsync(int companyId);
        Task<IEnumerable<Dto.Audit.CustomerDto>> LoadActiveCustomersAsync(int companyId);
        Task<IEnumerable<AuditScheduleDto>> LoadScheduledAuditNosAsync(
     string connectionStringName, int companyId, int financialYearId, int customerId);
        Task<IEnumerable<ReportTypeDto>> LoadAllReportTypeDetailsDRLAsync(
    string connectionStringName, int companyId, int templateId, string auditNo);


        Task<IEnumerable<CustomerUserEmailDto>> GetCustAllUserEmailsAsync(
        string connectionStringName, int companyId, int customerId);
        Task<IEnumerable<YearDto>> GetAddYearTo2DigitFinancialYearAsync(
    string connectionStringName, int companyId, int incrementBy);
        Task<int> GetDuringSelfAttachIdAsync(
    string connectionStringName, int companyId, int yearId, int customerId, int auditId, int drlId);
        Task<IEnumerable<DrlDescListDto>> LoadAllDRLDescriptionsAsync(string connectionStringName, int companyId);
         Task<DrlDescReqDto> LoadDRLDescriptionAsync(string connectionStringName, int companyId, int drlId);
        // Task<List<AttachmentDto>> LoadAttachmentsAsync(string connectionStringName, int companyId, int attachId, string dateFormat);
        Task<List<AttachmentDto>> LoadAttachmentsAsync(string connectionStringName, int companyId, int attachId);



Task<string> UploadAndSaveAttachmentAsync(AddFileDto dto);
        Task<List<LOEHeadingDto>> LoadLOEHeadingAsync(string sFormName, int compId, int reportTypeId, int loeTemplateId);


        Task<IEnumerable<WorkpaperNoDto>> GetAuditWorkpaperNosAsync(string connectionStringName, int companyId, int auditId);
        Task<IEnumerable<ChecklistItemDto>> LoadWorkpaperChecklistsAsync(string connectionStringName, int companyId);
        Task<IEnumerable<DRLAttachmentDto>> LoadOnlyDRLWithAttachmentsAsync(string connectionStringName, int companyId, string categoryType, string auditNo, int auditId);
        Task<bool> CheckWorkpaperRefExists(int auditId, string workpaperRef, int? workpaperId);
        Task<string> GenerateWorkpaperNo(int auditId);
        Task<int> GetNextWorkpaperIdAsync();
        // Task<int> SaveWorkpaper(Dto.Audit.WorkpaperDto dto, string workpaperNo);
        Task<int> SaveWorkpaperAsync(Dto.Audit.WorkpaperDto dto, string workpaperNo);
        Task<IEnumerable<WorkpaperViewDto>> LoadConductAuditWorkPapersAsync(int companyId, int auditId);
        Task<IEnumerable<StandardAuditHeadingDto>> LoadAllStandardAuditHeadingsAsync(int companyId, int auditId);
        Task<IEnumerable<WorkpaperNoDto>> GetConductAuditWorkpaperNosAsync(int companyId, int auditId);
        Task AssignWorkpaperToCheckPointAsync(AssignWorkpaperDto dto);
        Task<IEnumerable<AuditCheckPointDto>> LoadSelectedAuditCheckPointDetailsAsync(
    int companyId, int auditId, int empId, bool isPartner, int headingId, string heading);

        Task UpdateScheduleCheckPointRemarksAnnexureAsync(UpdateScheduleCheckPointDto dto);
        Task<string> UploadAndSaveAttachmentsAsync(AddFileDto dto);
    Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndLogDRLReportAsync(DRLRequestDto request, string format);
        Task<CustomerInvoiceDto> GetCustomerDetailsForInvoiceAsync(int companyId, int customerId);
        Task<CustomerDataDto> GetCustomerDetailsWithTemplatesAsync(int companyId, int customerId, int reportTypeId);
        Task<(string WordFilePath, string PdfFilePath)> GenerateCustomerReportFilesAsync(int companyId, int customerId, int reportTypeId);
        Task<IEnumerable<DRLAttachmentInfoDto>> GetDRLAttachmentInfoAsync(int compId, int customerId, int drlId);
        Task<int> SaveDRLLogWithAttachmentAsync(DRLLogDto dto, string filePath, string fileType);
        Task<string> GetLoeTemplateSignedOnAsync(
     string connectionStringName, int companyId, int auditTypeId, int customerId, int yearId, string dateFormat);
        Task<string> GetCustomerFinancialYearAsync(string connectionKey, int companyId, int customerId);

        Task<IEnumerable<ReportData>> GetReportTypesAsync(string connectionKey, int companyId);
        Task<string> GetDateFormatAsync(string connectionKey, int companyId, string configKey);
        //Task<List<int>> SaveLoETemplateDetailsAsync(string connectionKey, int companyId, List<LoETemplateDetailDto> details);
        Task<(int Id, string Action)> SaveOrUpdateLOETemplateDetailsAsync(string connectionKey, LoETemplateDetailInputDto dto);
        Task<IEnumerable<DropDownListDto>> LoadDRLClientSideAsync(int compId, string type, string auditNo);       
        Task<int> GetDuringSelfAttachIdAsync(int companyId, int yearId, int customerId, int auditId, int drlId);
    }


}
