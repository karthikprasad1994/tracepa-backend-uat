namespace TracePca.Dto.FIN_Statement
{
    public class LedgerDifferenceDto
    {
        //GetDescriptionWiseDetails
        public class DescriptionWiseDetailsDto
        {
            public int HeadingId { get; set; }
            public string HeadingName { get; set; }
            public string Status { get; set; }
            public decimal CYamt { get; set; }
            public decimal PYamt { get; set; }
            public decimal Difference_Amt { get; set; }
            public decimal Difference_Avg { get; set; }
            public string RiskFactor { get; set; }
            public string Materiality { get; set; }
            public decimal cyCr { get; set; }
            public decimal cyDb { get; set; }
            public decimal pyCr { get; set; }
            public decimal pyDb { get; set; }
        }

        //UpdateDescriptionWiseDetailsStatus
        public class UpdateDescriptionWiseDetailsStatusDto
        {
            public int Id { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        //GetDescriptionDetails
        public class DescriptionDetailsDto
        {
            public string HeadingName { get; set; }     
                        public int HeadingId { get; set; }
                 public decimal CYamt { get; set; }
            public decimal PYamt { get; set; }
            public decimal Difference_Amt { get; set; }
            public decimal? Difference_Avg { get; set; }

            public decimal CyCr { get; set; }
            public decimal CyDb { get; set; }
            public decimal PyCr { get; set; }
            public decimal PyDb { get; set; }
            public string status { get; set; }
        }

        //GetVODTotalGrid
        public class CustCOATrialBalanceResult
        {
            public List<dynamic> MainTrailBalance { get; set; }
            public List<dynamic> UnmappedCustomerUpload { get; set; }
            public dynamic CustomerTotals { get; set; }
            public List<dynamic> SystemTotals { get; set; }
        }
    }
}
