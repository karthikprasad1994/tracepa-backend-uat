using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TracePca.Dto.Audit;
using static TracePca.Dto.Dashboard.DashboardDto;

namespace TracePca.Interface.Dashboard
{
    public interface DashboardInterface
    {
        Task<RemarksSummaryDto> GetRemarksSummaryAsync();
        Task<IEnumerable<StandardAuditF1DTO>> GetStandardAuditsAsync();
        Task<IEnumerable<StandardAuditF2DTO>> GetStandardAuditsFramework0Async();
        Task<LOEStatusSummary> GetLOEProgressAsync(int compId, int yearId, int custId);
        Task<AuditStatusSummary> GetAuditProgressAsync(int compId, int yearId, int custId);
        Task<PassedDueDatesSummary> GetAuditPassedDueDatesAsync(int compId, int yearId, int custId);
    }
}
