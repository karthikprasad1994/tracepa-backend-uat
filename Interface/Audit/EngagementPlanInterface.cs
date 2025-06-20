using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface EngagementPlanInterface
    {
        Task<AuditDropDownListDataDTO> LoadAllDDLDataAsync(int compId);
        Task<AuditDropDownListDataDTO> LoadEngagementPlanDDLAsync(int compId, int yearId, int custId);
        Task<IEnumerable<ReportTypeDetailsDTO>> GetReportTypeDetails(int compId, int reportTypeId);
        Task<EngagementPlanDetailsDTO> CheckAndGetEngagementPlanByIdsAsync(int compId, int customerId, int yearId, int auditTypeId);
        Task<EngagementPlanDetailsDTO> GetEngagementPlanByIdAsync(int compId, int epPKid);
        Task<int> SaveOrUpdateEngagementPlanDataAsync(EngagementPlanDetailsDTO dto);
        Task<bool> ApproveEngagementPlanAsync(int compId, int epPKid, int approvedBy);
        Task<List<AttachmentDetailsDTO>> LoadAllAttachmentsByIdAsync(int compId, int attachId);
        Task<(int attachmentId, string relativeFilePath)> UploadAndSaveAttachmentAsync(FileAttachmentDTO dto);
        Task RemoveAttachmentDocAsync(int compId, int attachId, int docId, int userId);
        Task UpdateAttachmentDocDescriptionAsync(int compId, int attachId, int docId, int userId, string description);
        Task<AttachmentDetailsDTO> GetAttachmentDocDetailsByIdAsync(int compId, int attachId, int docId);
        Task<AuditDropDownListDataDTO> LoadUsersByCustomerIdDDLAsync(int custId);
        Task<EngagementPlanReportDetailsDTO> GetEngagementPlanReportDetailsByIdAsync(int compId, int epPKid);
        Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadReportAsync(int compId, int epPKid, string format);
        Task<string> GenerateReportAndGetURLPathAsync(int compId, int epPKid, string format);
        Task<bool> SendEmailAndSaveEngagementPlanExportDataAsync(EngagementPlanReportExportDetailsDTO dto);
    }
}
