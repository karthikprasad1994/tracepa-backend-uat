using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface EngagementPlanInterface
    {
        Task<AuditDropDownListDataDTO> LoadAllDDLDataAsync(int compId);
        Task<AuditDropDownListDataDTO> LoadEngagementPlanDDLAsync(int compId, int yearId, int custId);
        Task<IEnumerable<ReportTypeDetails>> GetReportTypeDetails(int compId, int reportTypeId);
        Task<EngagementDetailsDTO> CheckAndGetEngagementPlanByIdsAsync(int compId, int customerId, int yearId, int auditTypeId);
        Task<EngagementDetailsDTO> GetEngagementPlanByIdAsync(int compId, int epPKid);
        Task<int> SaveOrUpdateEngagementPlanDataAsync(EngagementDetailsDTO dto);
        Task<bool> ApproveEngagementPlanAsync(int compId, int epPKid, int approvedBy);
    }
}
