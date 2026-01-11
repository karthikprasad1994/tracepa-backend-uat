using TracePca.Dto.Audit;

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
        public class CashFlowParticularDto
        {
            public int Sr_No { get; set; }
            public string ParticularName { get; set; } = string.Empty;
            public decimal CurrentYear { get; set; } = 0m;
            public decimal PreviousYear { get; set; } = 0m;
        }
        public class CashFlowCategory1Result
        {
            public List<CashFlowParticularDto> Particular { get; set; } = new List<CashFlowParticularDto>();
        }
        public class UserAdjustmentInput
        {
            public string Description { get; set; } = string.Empty;
            public decimal CurrentYear { get; set; } = 0m;
            public decimal PreviousYear { get; set; } = 0m;
        }

        public class CashflowClientDto
        {
            public int Cust_Id { get; set; }
            public string Cust_Name { get; set; }
        }

        public class CashflowBranchDto
        {
            public int BranchId { get; set; }
            public string BranchName { get; set; }
        }

        public class CashflowFinacialYearDto
        {
            public int YMS_YEARID { get; set; }
            public string YMS_ID { get; set; }
        }

        public class CashFlowAddDto
        {
            public string ACF_Description { get; set; }
            public int ACF_Custid { get; set; }
            public int ACF_Branchid { get; set; }
            public int ACF_Yearid { get; set; }
            public int ACF_Compid { get; set; }
        }


        public class CashFlowOperatingSystemDto
        {
            public int SrNo { get; set; }
            public int CashFlowId { get; set; }
            public string Particulars { get; set; }
            public decimal PreviousAmount { get; set; }
            public decimal CurrentAmount { get; set; }
        }
    }
}

