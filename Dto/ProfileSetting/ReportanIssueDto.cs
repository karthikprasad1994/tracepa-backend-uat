namespace TracePca.Dto.ProfileSetting
{
    public class ReportanIssueDto
    {
        public class IssueReportDto
        {
            public string Base64Image { get; set; }
            public string EmailText { get; set; }
            public int userid { get; set; }
            public string accessCode { get; set; }
        }
        public class UserDetailsDto
        {
            public string usr_FullName { get; set; }
            public string usr_LoginName { get; set; }
        }


    }
}
