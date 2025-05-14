using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditCompletionInterface
    {
        Task<AuditDropDownListDataDTO> LoadAllAuditDDLDataAsync(int compId);
        Task<IEnumerable<ReportTypeDetailsDTO>> GetReportTypeDetails(int compId);
        Task<AuditDropDownListDataDTO> LoadAuditNoDDLAsync(int compId, int yearId, int custId, int userId);
        Task<AuditDropDownListDataDTO> LoadAuditWorkPaperDDLAsync(int compId, int auditId);
        Task<AuditCompletionDTO> GetAuditCompletionDetailsByIdAsync(int compId, int auditId);
        Task<List<AuditCompletionSubPointDetailsDTO>> GetAuditCompletionSubPointDetailsAsync(int compId, int auditId, int checkPointId);
        Task<int> SaveOrUpdateAuditCompletionDataAsync(AuditCompletionDTO dto);
        Task<int> UpdateSignedByUDINInAuditAsync(AuditSignedByUDINRequestDTO dto);
    }
}
