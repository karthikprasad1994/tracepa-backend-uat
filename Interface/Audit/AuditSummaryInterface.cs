using System.Net.Mail;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Service.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditSummaryInterface
    {
        Task<DropDownDataDto> LoadCustomerDataAsync(int compId);

        Task<DropDownDataDto> LoadAuditNoDataAsync(int custId, int compId, int financialYearId, int loginUserId);

        Task<IEnumerable<AuditDetailsDto>> GetAuditDetailsAsync(int compId, int customerId, int auditNo);

        Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryAsync(int compId, int customerId, int auditNo, int yearId);

        Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryDuringAuditAsync(int compId, int customerId, int auditNo, int requestId, int yearId);

        Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryCompletionAuditAsync(int compId, int customerId, int auditNo, int requestId, int yearId);

        Task<IEnumerable<AuditProgramSummaryDto>> GetAuditProgramSummaryAsync(int compId, int auditNo);

        Task<IEnumerable<WorkspaceSummaryDto>> GetWorkspaceSummaryAsync(int compId, int auditNo);

        Task<IEnumerable<CMADto>> GetCAMDetailsAsync(int compId, int auditNo);

        Task<bool> UpdateStandardAuditASCAMdetailsAsync(int sacm_pkid, int sacm_sa_id, UpdateStandardAuditASCAMdetailsDto dto);

        Task<string> UploadCMAAttachmentsAsync(CMADtoAttachment dto);

        Task<IEnumerable<CAMAttachmentDetailsDto>> GetCAMAttachmentDetailsAsync(int AttachID);

        //Task<IEnumerable<CAMAttachmentDetailsDto>> GetCAMAttachmentDetailsAsync(int AttachID, CAMAttachmentDetailsDto dto);
    }
}
