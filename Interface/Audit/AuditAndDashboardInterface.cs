using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditAndDashboardInterface
    {
        Task<List<DashboardAndScheduleDto>> GetDashboardAuditAsync(
             int? id, int? customerId, int? compId, int? financialYearId, int? loginUserId);
        Task<List<UserDto>> GetUsersByRoleAsync(int compId, string role);
        Task<List<AuditTypeCustomerDto>> GetAuditTypesByCustomerAsync(int compId, string sType, int custId, int fyId, int auditTypeId);
        List<QuarterDto> GenerateQuarters(DateTime? fromDate);
        Task<List<AuditTypeHeadingDto>> LoadAllAuditTypeHeadingsAsync(int compId, int auditTypeId);
        Task<List<AssignedCheckpointDto>> GetAssignedCheckpointsAndTeamMembersAsync(
          int compId, int auditId, int AuditTypeId, int custId, string heading = "", string? sCheckPoints = null);

        Task<int> InsertStandardAuditScheduleWithQuartersAsync(string sAC, StandardAuditScheduleDto dto, int custRegAccessCodeId, int iUserID, string sIPAddress, string sModule, string sForm, string sEvent, int iMasterID, string sMasterName, int iSubMasterID, string sSubMasterName);
        Task<string[]> SaveUpdateStandardAuditChecklistDetailsAsync(StandardAuditChecklistDetailsDto objSACLD);

        Task<int> SaveOrUpdateFullAuditSchedule(StandardAuditScheduleDTO dto);
        Task<IEnumerable<UserDto>> GetUsersAsync(int companyId, string userIds);
        Task<IEnumerable<AuditChecklistDto>> GetChecklistAsync(int auditId, int companyId);
        Task<bool> DeleteSelectedCheckpointsAndTeamMembersAsync(DeleteCheckpointDto dto);

        Task<int[]> SaveCustomerMasterAsync(int iCompId, AuditCustomerDetailsDto dto);
    }
}
