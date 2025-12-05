using System.Data;

namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleAccountingRatioDto
    {
        public class AccountingRatioResult
        {
            public List<RatioDto> Ratios { get; set; } = new();
            public DataTable DataTable { get; set; }
        }

        public class RatioDto
        {
            public string Sr_No { get; set; }
            public string Ratios { get; set; }
            public string Numerator { get; set; }
            public string Denominator { get; set; }
            public decimal Current_Reporting_Period { get; set; }
            public decimal Previous_Reporting_Period { get; set; }
            public decimal Change { get; set; }
        }

        public class AmountDto
        {
            public decimal Dc1 { get; set; }
            public decimal DP1 { get; set; }
        }
    }
}
