using TracePca.Dto;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Master
{
    public interface ContentManagementMasterInterface
    {
        Task<(bool Success, string Message, MasterDropDownListDataDTO? Data)> LoadAllMasterDDLDataAsync(int compId);
        Task<(bool Success, string Message, List<ContentManagementMasterDTO> Data)> GetMasterDataByStatusAsync(string type, string status, int compId);
        Task<(bool Success, string Message, List<string>? Data)> GetActMasterDataAsync(int compId);
        Task<(bool Success, string Message, ContentManagementMasterDTO? Data)> GetMasterDataByIdAsync(int id, int compId);
        Task<(int Id, string Message, List<ContentManagementMasterDTO> MasterList)> SaveOrUpdateMasterDataAndGetRecordsAsync(ContentManagementMasterDTO dto);
        Task<(bool Success, string Message)> UpdateMasterRecordsStatusAsync(List<int> ids, string action, int compId, int updatedBy, string ipAddress);

        Task<(bool Success, string Message, List<string>? Data)> GetAuditTypeChecklistHeadingDataAsync(int compId);
        Task<(bool Success, string Message, List<AuditTypeChecklistMasterDTO> Data)> GetAuditTypeChecklistByStatusAsync(int typeId, string status, int compId);
        Task<(bool Success, string Message, AuditTypeChecklistMasterDTO? Data)> GetAuditTypeChecklistByIdAsync(int id, int compId);
        Task<(int Id, string Message, List<AuditTypeChecklistMasterDTO> MasterList)> SaveOrUpdateAuditTypeChecklistAndGetRecordsAsync(AuditTypeChecklistMasterDTO dto);
        Task<(bool Success, string Message)> UpdateAuditTypeChecklistStatusAsync(List<int> ids, string action, int compId, int userId, string ipAddress);

        Task<(bool Success, string Message, List<string>? Data)> GetAssignmentTaskChecklistHeadingDataAsync(int compId);
        Task<(bool Success, string Message, List<AssignmentTaskChecklistMasterDTO> Data)> GetAssignmentTaskChecklistByStatusAsync(int taskId, string status, int compId);
        Task<(bool Success, string Message, AssignmentTaskChecklistMasterDTO? Data)> GetAssignmentTaskChecklistByIdAsync(int id, int compId);
        Task<(int Id, string Message, List<AssignmentTaskChecklistMasterDTO> MasterList)> SaveOrUpdateAssignmentTaskChecklistAndGetRecordsAsync(AssignmentTaskChecklistMasterDTO dto);
        Task<(bool Success, string Message)> UpdateAssignmentTaskChecklistStatusAsync(List<int> ids, string action, int compId, int userId, string ipAddress);

        Task<(bool Success, string Message, List<AuditCompletionSubPointMasterDTO> Data)> GetAuditSubPointsByStatusAsync(int checkPointId, string status, int compId);
        Task<(bool Success, string Message, AuditCompletionSubPointMasterDTO? Data)> GetAuditSubPointByIdAsync(int id, int compId);
        Task<(int Id, string Message, List<AuditCompletionSubPointMasterDTO> MasterList)> SaveOrUpdateAuditSubPointAndGetRecordsAsync(AuditCompletionSubPointMasterDTO dto);
        Task<(bool Success, string Message)> UpdateAuditSubPointStatusAsync(List<int> ids, string action, int compId, int userId, string ipAddress);
    }
}
