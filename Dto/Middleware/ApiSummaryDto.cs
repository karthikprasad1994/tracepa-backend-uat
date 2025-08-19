namespace TracePca.Dto.Middleware
{
    public class ApiSummaryDto
    {
        public string FormName { get; set; }
        public string ApiName { get; set; }
        public double AvgResponseTimeMs { get; set; }
        public double MaxResponseTimeMs { get; set; }
        public double LatestResponseTimeMs { get; set; }
        public DateTime LastSeen { get; set; }

        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        // public int UserId { get; set; }


    }
}
