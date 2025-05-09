using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface EngagementPlanInterface
    {
        Task<AuditDropDownListDataDTO> LoadAllDDLDataAsync(int compId);
        Task<AuditDropDownListDataDTO> LoadEngagementPlanDDLAsync(int compId, int yearId, int custId);
        Task<IEnumerable<ReportTypeDetails>> GetReportTypeDetails(int compId, int reportTypeId);
        Task<EngagementPlanDetailsDTO> CheckAndGetEngagementPlanByIdsAsync(int compId, int customerId, int yearId, int auditTypeId);
        Task<EngagementPlanDetailsDTO> GetEngagementPlanByIdAsync(int compId, int epPKid);
        Task<int> SaveOrUpdateEngagementPlanDataAsync(EngagementPlanDetailsDTO dto);
        Task<bool> ApproveEngagementPlanAsync(int compId, int epPKid, int approvedBy);
    }
}
