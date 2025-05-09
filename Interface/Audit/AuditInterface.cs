using TracePca.Dto;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditInterface
    {
        Task<CustomerAuditDropdownDto> GetCustomerAuditDropdownAsync(int companyId);
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

         Task<DrlDescReqDto> LoadDRLDescriptionAsync(string connectionStringName, int companyId, int drlId);
        Task<List<AttachmentDto>> LoadAttachmentsAsync(string connectionStringName, int companyId, int attachId, string dateFormat);
      
        
Task<string> UploadAndSaveAttachmentAsync(AddFileDto dto);
        



    }
}