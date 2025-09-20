namespace TracePca.Dto.FIN_Statement
{
    public class LedgerDifferenceDto
    {

        //GetHeadWiseDetails
        public class HeadWiseDetailsDto
        {
            public int HeadingId { get; set; }
            public string HeadingName { get; set; }
            public decimal CYamt { get; set; }
            public decimal PYamt { get; set; }
            public decimal Difference_Amt { get; set; }
            public decimal? Difference_Avg { get; set; }
            public string RiskFactor { get; set; }
            public string Materiality { get; set; }
        }

        //GetAccountWiseDetails
        public class AccountWiseDetailsDto
        {
            public string HeadingName { get; set; }
            public decimal CYamt { get; set; }
            public int ASH_Notes { get; set; }
            public decimal PYamt { get; set; }
        }

        //GetDescriptionWiseDetails
        public class DescriptionWiseDetailsDto
        {
            public int HeadingId { get; set; }
            public string HeadingName { get; set; }
            public decimal CYamt { get; set; }
            public decimal PYamt { get; set; }
            public decimal Difference_Amt { get; set; }
            public decimal Difference_Avg { get; set; }
            public string RiskFactor { get; set; }
            public string Materiality { get; set; }
        }

    }
}
