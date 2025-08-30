using System.Data;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditAndDashboardInterface
    {
        Task<List<DashboardAndScheduleDto>> GetDashboardAuditAsync(
             int? id, int? customerId, int? compId, int? financialYearId, int? loginUserId);
        Task<List<UserDto>> GetUsersByRoleAsync(int compId, string role);
        Task<List<AuditTypeCustomerDto>> LoadAuditTypeComplianceDetailsAsync(AuditTypeRequestDto req);
        List<QuarterDto> GenerateQuarters(DateTime? fromDate);
        Task<List<AuditTypeHeadingDto>> LoadAllAuditTypeHeadingsAsync(int compId, int auditTypeId);
        Task<List<AssignedCheckpointDto>> GetAssignedCheckpointsAndTeamMembersAsync(
               int compId, int auditId, int auditTypeId, int custId,
               string sType, string heading, string? sCheckPoints);

        Task<int> InsertStandardAuditScheduleWithQuartersAsync(string sAC, StandardAuditScheduleDto dto, int custRegAccessCodeId, int iUserID, string sIPAddress, string sModule, string sForm, string sEvent, int iMasterID, string sMasterName, int iSubMasterID, string sSubMasterName);
        Task<string[]> SaveUpdateStandardAuditChecklistDetailsAsync(StandardAuditChecklistDetailsDto objSACLD);

        Task<int> SaveOrUpdateFullAuditSchedule(StandardAuditScheduleDTO dto);
        Task<IEnumerable<UserDto>> GetUsersAsync(int companyId, string userIds);
        Task<IEnumerable<AuditChecklistDto>> GetChecklistAsync(int auditId, int companyId);
        Task<bool> DeleteSelectedCheckpointsAndTeamMembersAsync(DeleteCheckpointDto dto);

        Task<int[]> SaveCustomerMasterAsync(int iCompId, AuditCustomerDetailsDto dto);
        Task<IEnumerable<AuditChecklistDto>> LoadAuditTypeCheckListAsync(
        int compId,
        int auditId,
        int auditTypeId,
        string heading);
        Task<List<AssignedCheckpointDto>> GetAssignedCheckpointsAsync(int auditId, int custId, string heading);
        Task<IEnumerable<GeneralMasterDto>> LoadGeneralMastersAsync(int iACID, string sType);
        Task<CustomerDetailsDto> GetCustomerDetailsAsync(int iACID, int iCustId);
        Task<AuditStatusDto> GetAuditStatusAsync(int saId, int companyId);
        Task<bool> CheckScheduleQuarterDetailsAsync(ScheduleQuarterCheckDto dto);
        Task<bool> IsCustomerLoeApprovedAsync(int customerId, int yearId);
        Task<string> GetLOESignedOnAsync(int compid, int auditTypeId, int customerId, int yearId);
        Task<string> GetLOEStatusAsync(int compid, int auditTypeId, int customerId, int yearId);
        Task<(DateTime? StartDate, DateTime? EndDate)> GetScheduleQuarterDateDetailsAsync(int iAcID, int iAuditID, int iQuarterID);

        Task<dynamic?> LoadCustomerMasterAsync(int companyId, int customerId);
        Task<string[]> SaveEmployeeDetailsAsync(EmployeeDto employee);
        Task<object> LoadExistingEmployeeDetailsAsync(int companyId, int userId);

        Task<IEnumerable<dynamic>> LoadActiveRoleAsync(int companyId);
        Task<IEnumerable<dynamic>> GetUsersByCompanyAndRoleAsync(int companyId, int usrRole);

        Task<DataTable> LoadAuditScheduleIntervalAsync(int accessCodeId, int auditId, string format);
        Task<DataTable> LoadAssignedCheckPointsAndTeamMembersAsync(int accessCodeId, int auditId, int customerId, string heading, string format);
        Task<DataTable> GetFinalAuditTypeHeadingsAsync(int accessCodeId, int auditId);

        Task<string> GetUserNamesAsync(int accessCodeId, List<int> engagementPartnerIds);
        Task<string> GetUserNames1Async(int accessCodeId, List<int> engagementPartnerIds);
        Task<string> GetUserNames2Async(int accessCodeId, List<int> engagementPartnerIds);
        Task<string> GetUserNames3Async(int accessCodeId, List<int> engagementPartnerIds);

        string GetFormattedDate(string accessCode, int accessCodeId);

        Task<IEnumerable<CustomerDto1>> GetCustomersAsync(int companyId);

        Task<DiscoveryResponseDto> GetAnswerAsync(string question);

            Task<LoeAuditFrameworkResponse> GetLoeAuditFrameworkIdAsync(LoeAuditFrameworkRequest request);

        Task<AuditStatusDto> GetAuditCompStatusAsync(int compId, int saId);


    }
}
