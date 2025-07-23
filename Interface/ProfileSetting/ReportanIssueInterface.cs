using static TracePca.Dto.ProfileSetting.ReportanIssueDto;


namespace TracePca.Interface.ProfileSetting
{
    public interface ReportanIssueInterface
    {
        Task<bool> ReportIssueAsync(IssueReportDto issueDto, string userFullName, string userLogin, string accessCode);
        Task<UserDetailsDto> GetUserDetailsAsync(int userId);


    }
}
