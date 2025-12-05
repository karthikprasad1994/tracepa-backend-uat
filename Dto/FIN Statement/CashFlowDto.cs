namespace TracePca.Dto.FIN_Statement
{
    public class CashFlowDto
    {

        //DeleteCashFlowCategoryWise
        public class DeleteCashflowCategoryWiseDto
        {
            public int CompId { get; set; }
            public int PkId { get; set; }
            public int CustId { get; set; }
            public int Category { get; set; }
        }

        //GetCashFlowID(SearchButton)
        public class GetCashflowPkidDto
        {
            public int CompId { get; set; }
            public string Description { get; set; }
            public int CustId { get; set; }
            public int BranchId { get; set; }
        }

        //GetCashFlowForAllCategory
        public class CashFlowForAllCategoryDto
        {
            public int ACF_pkid { get; set; }
            public int ACF_Custid { get; set; }
            public string ACF_Description { get; set; }
            public decimal ACF_Current_Amount { get; set; }
            public decimal ACF_Prev_Amount { get; set; }
        }

        //SaveCashFlow(Category 1)
        //public class CashflowParticularDto
        //{
        //    public string Description { get; set; }         
        //    public bool IsHeading { get; set; }              
        //    public decimal Amount { get; set; }            
        //    public string SourceHint { get; set; }           
        //    public string Note { get; set; }                 
        //}
        //public class CashflowMandatoryResponseDto
        //{
        //    public bool HasCashflow { get; set; }                     
        //    public List<CashflowParticularDto> Particulars { get; set; } = new();
        //}
        //public class HeadingAmountDto
        //{
        //    public decimal Dc1 { get; set; } = 0m;       
        //    public decimal DP1 { get; set; } = 0m;       
        //}

        public class CashFlowCategory1Result
        {
            public List<CashFlowParticularDto> Particular { get; set; } = new List<CashFlowParticularDto>();
            public Dictionary<string, object> Diagnostics { get; set; } = new Dictionary<string, object>();
        }
        public class CashFlowParticularDto
        {
            public int Sr_No { get; set; }
            public string ParticularName { get; set; }
            public decimal CurrentYear { get; set; }
            public decimal PreviousYear { get; set; }
        }
        public class HeadingAmount
        {
            public decimal Dc1 { get; set; }
            public decimal DP1 { get; set; }
        }
    }
}

