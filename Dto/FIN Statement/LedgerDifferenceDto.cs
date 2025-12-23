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

        //GetCustomerTBGrid
        //public class CustCOATrialBalanceResult
        //{
        //    public List<dynamic> MainTrailBalance { get; set; }
        //    public List<dynamic> UnmappedCustomerUpload { get; set; }
        //    public dynamic CustomerTotals { get; set; }
        //    public List<dynamic> SystemTotals { get; set; }
        //}
        //public class GetCustomerTBGridDto
        //{
        //    public int SrNo { get; set; }
        //    public int DescID { get; set; }
        //    public int DescDetailsID { get; set; }
        //    public string DelFlg { get; set; }
        //    public int ATBCU_CustId { get; set; }
        //    public string DescriptionCode { get; set; }
        //    public decimal CustOpeningDebit { get; set; }
        //    public decimal CustOpeningCredit { get; set; }
        //    public decimal CustTrDebit { get; set; }
        //    public decimal CustTrCredit { get; set; }
        //    public decimal CustClosingDebit { get; set; }
        //    public decimal CustClosingCredit { get; set; }
        //    public int SubItemID { get; set; }
        //    public string ASSI_Name { get; set; }
        //    public int ItemID { get; set; }
        //    public string ASI_Name { get; set; }
        //    public int SubHeadingID { get; set; }
        //    public string ASSH_Name { get; set; }
        //    public int HeadingID { get; set; }
        //    public string ASH_Name { get; set; }
        //    public string Status { get; set; }
        //    public string CompanyType { get; set; }
        //    public int ScheduleType { get; set; }
        //    public decimal OpeningDebit { get; set; }
        //    public decimal OpeningCredit { get; set; }
        //    public decimal TrDebit { get; set; }
        //    public decimal TrCredit { get; set; }
        //    public decimal ClosingDebit { get; set; }
        //    public decimal ClosingCredit { get; set; }
        //}

        public class CustCoaRequestDto
        {
            public int AccessCodeId { get; set; }       // Session("AccessCodeID")
            public int CustomerId { get; set; }
            public int FinancialYearId { get; set; }
            public int ScheduleTypeId { get; set; }
            public int Unmapped { get; set; }
            public int BranchId { get; set; }
        }

        //UpdateCustomerTBDelFlg
        public class UpdateCustomerTrailBalanceStatusDto
        {
            public int Id { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        public class CustCoaRequestFlaggedDto
        {
            public int CustId { get; set; }
            public int YearId { get; set; }
            public int BranchId { get; set; }
        }

    }
}
