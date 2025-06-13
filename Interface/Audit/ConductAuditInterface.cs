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
    }
}
