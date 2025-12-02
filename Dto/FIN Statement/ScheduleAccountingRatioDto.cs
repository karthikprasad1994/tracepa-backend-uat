using System.Data;

namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleAccountingRatioDto
    {
        public class RatioDto
        {
            public int Sr_No { get; set; }
            public string RatioName { get; set; }
            public string Numerator { get; set; }
            public string Denominator { get; set; }
            public decimal CurrentReportingPeriod { get; set; }
            public decimal PreviousReportingPeriod { get; set; }
            public decimal Change { get; set; }
        }

        public class AccountingRatioResult
        {
            public List<RatioDto> Ratios { get; set; } = new List<RatioDto>();
        }

        // small helper used internally by service
        public struct HeadingAmount
        {
            public decimal Dc1;
            public decimal DP1;
        }

    }
}
