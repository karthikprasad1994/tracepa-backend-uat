namespace TracePca.Dto.Dashboard
{
    public class DashboardDto
    {
        public class RemarksCountDto
        {
            public string? RemarkStatus { get; set; }
            public int RemarkCount { get; set; }
        }
        public class RemarksSummaryDto
        {
            public int Sent { get; set; }
            public int Received { get; set; }
        }
    }
}
