using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditCompletionInterface
    {
        Task<AuditDropDownListDataDTO> LoadAllAuditDDLDataAsync(int compId);
        Task<IEnumerable<ReportTypeDetails>> GetReportTypeDetails(int compId);
        Task<AuditDropDownListDataDTO> LoadAuditNoDDLAsync(int compId, int yearId, int custId, int userId);
        Task<AuditDropDownListDataDTO> LoadAuditWorkpaperDDLAsync(int compId, int auditId);
    }
}
