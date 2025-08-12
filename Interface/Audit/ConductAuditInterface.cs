using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface ConductAuditInterface
    {
        Task<AuditDropDownListDataDTO> LoadAllAuditDDLDataAsync(int compId);
        Task<string> GetCustomerFinancialYearAsync(int compId, int custId);
        Task<AuditDropDownListDataDTO> LoadAuditNoDDLAsync(int compId, int yearId, int custId, int userId);
        Task<AuditDropDownListDataDTO> LoadDRLWithAttachmentsDDLAsync(int compId, int auditId);
        Task<bool> IsDuplicateWorkpaperRefAsync(int auditId, string workpaperRef, int? workpaperId);
        Task<List<ConductAuditWorkPaperDetailsDTO>> GetConductAuditWorkPapersByAuditIdAsync(int compId, int auditId);
        Task<ConductAuditWorkPaperDetailsDTO> GetConductAuditWorkPaperByWPIdAsync(int compId, int auditId, int workpaperId);
        Task<int> SaveOrUpdateConductAuditWorkpaperAsync(ConductAuditWorkPaperDetailsDTO dto);
        Task<AuditDropDownListDataDTO> LoadConductAuditCheckPointHeadingsAsync(int compId, int auditId);
        Task<List<ConductAuditDetailsDTO>> LoadConductAuditCheckPointDetailsAsync(int compId, int auditId, int userId, string? heading);
        Task<bool> AssignWorkpapersToCheckPointsAsync(List<ConductAuditDetailsDTO> dtos);
        Task<bool> UpdateConductAuditCheckPointRemarksAndAnnexureAsync(ConductAuditDetailsDTO dto);
        Task<(int attachmentId, string relativeFilePath)> UploadAndSaveCheckPointAttachmentAsync(FileAttachmentDTO dto, int auditId, int checkPointId, string module);
        Task<(int attachmentId, string relativeFilePath)> UploadAndSaveWorkPaperAttachmentAsync(FileAttachmentDTO dto, int auditId, int checkPointId, string module);
        Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadWorkpapersReportAsync(int compId, int auditId, string format);
        Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadCheckPointsReportAsync(int compId, int auditId, string format);
        Task<string> GenerateWorkpapersReportAndGetURLPathAsync(int compId, int auditId, string format);
        Task<string> GenerateCheckPointsReportAndGetURLPathAsync(int compId, int auditId, string format);
        Task<AuditDropDownListDataDTO> LoadUsersByCustomerIdDDLAsync(int custId);
        Task<bool> UpdateConductAuditWorkPaperReviewerAsync(int compID, int auditId, int workpaperId, int userId, string reviewerComments);
        Task<List<ConductAuditRemarksHistoryDisplayDTO>> GetConductAuditRemarksHistoryAsync(int compId,int auditId, int conductAuditCheckPointPKId, int checkPointId);
        Task<int> SaveConductAuditRemarksHistoryAsync(ConductAuditRemarksHistoryDTO dto);
        Task<int> CheckAuditMandatoryCheckpointsAsync(int compId, int auditId);
        Task<bool> SaveConductAuditCheckpointObservationAsync(List<ConductAuditCheckpointObservationsDTO> dtos);
    }
}