using TracePca.Dto;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Master
{
    public interface ContentManagementMasterInterface
    {
        Task<(int Id, string Message)> SaveOrUpdateAuditFrequencyDataAsync(ContentManagementMasterDTO dto);
        Task<(int Id, string Message)> SaveOrUpdateEngagementFeesDataAsync(ContentManagementMasterDTO dto);
        Task<(int Id, string Message)> SaveOrUpdateTypeofReportDataAsync(ContentManagementMasterDTO dto);
        Task<(int Id, string Message)> SaveOrUpdateTypeofTestDataAsync(ContentManagementMasterDTO dto);
        Task<(int Id, string Message)> SaveOrUpdateManagementRepresentationsDataAsync(ContentManagementMasterDTO dto);
        Task<(int Id, string Message)> SaveOrUpdateAuditTaskOrAssignmentTaskDataAsync(ContentManagementMasterDTO dto);
        Task<(int Id, string Message)> SaveOrUpdateWorkpaperChecklistMasterDataAsync(ContentManagementMasterDTO dto);
        Task<(int Id, string Message)> SaveOrUpdateAuditCompletionCheckPointsDataAsync(ContentManagementMasterDTO dto);
    }
}
