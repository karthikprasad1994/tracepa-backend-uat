namespace TracePca.Dto.Application_Metric
{
    public class ApplicationMetricDto
    {
        public List<AvgLoadTimeDto> AvgLoadTimeByForm { get; set; }
        public UserActivityStatusDto UserActivityStatus { get; set; }
        public List<RequestThroughputDto> RequestThroughput { get; set; }
        public SummaryStatsDto SummaryStats { get; set; }
        public List<ErrorBreakdownDto> ErrorBreakdown { get; set; }
    }


    public class AvgLoadTimeDto
    {
        public string FormName { get; set; }
        public int AvgLoadTime { get; set; }
    }

    public class UserActivityStatusDto
    {
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
    }

    public class RequestThroughputDto
    {
        public string TimeSlot { get; set; }
        public int RequestCount { get; set; }
    }

    public class SummaryStatsDto
    {
        public int AvgResponseTimeMs { get; set; }
        public double ErrorRatePercent { get; set; }
        public int ActiveUserCount { get; set; }
    }

    public class ErrorBreakdownDto
    {
        public string ErrorType { get; set; }
        public int Count { get; set; }
    }
}
