namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleAccountingRatioDto
    {
        public class RatioDto
        {
           
            public string SrNo { get; set; }
            public string Ratio { get; set; }
            public string Numerator { get; set; }
            public string Denominator { get; set; }
            public string CurrentReportingPeriod { get; set; }
            public string PreviousReportingPeriod { get; set; }
            public string Change { get; set; }
        }

    }
}
