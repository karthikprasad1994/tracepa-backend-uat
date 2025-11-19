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
        public class CashFlowCategory1
        {
            public int ACF_pkid { get; set; }
            public string ACF_Description { get; set; }
            public int ACF_Custid { get; set; }
            public int ACF_Branchid { get; set; }
            public double ACF_Current_Amount { get; set; }
            public double ACF_Prev_Amount { get; set; }
            public string ACF_Status { get; set; }
            public int ACF_Crby { get; set; }
            public int ACF_Updatedby { get; set; }
            public int ACF_Compid { get; set; }
            public string ACF_Ipaddress { get; set; }
            public int ACF_Catagary { get; set; }
            public int ACF_yearid { get; set; }
            public bool IsDeleted { get; internal set; }
        }

        //SaveCashFlow(Category3)
        public class CashFlowCategory3
        {
            public int ACF_pkid { get; set; }
            public string ACF_Description { get; set; }
            public int ACF_Custid { get; set; }
            public int ACF_Branchid { get; set; }
            public decimal? ACF_Current_Amount { get; set; }
            public decimal? ACF_Prev_Amount { get; set; }
            public string ACF_Status { get; set; }
            public int ACF_Crby { get; set; }
            public int ACF_Updatedby { get; set; }
            public string ACF_Ipaddress { get; set; }
            public int ACF_Compid { get; set; }
            public int ACF_Catagary { get; set; }
            public int ACF_yearid { get; set; }
        }

        //SaveCashFlow(Category4)
        public class CashFlowCategory4
        {
            public int ACF_pkid { get; set; }
            public string ACF_Description { get; set; }
            public int ACF_Custid { get; set; }
            public int ACF_Branchid { get; set; }
            public double ACF_Current_Amount { get; set; }
            public double ACF_Prev_Amount { get; set; }
            public string ACF_Status { get; set; }
            public int ACF_Crby { get; set; }
            public int ACF_Updatedby { get; set; }
            public int ACF_Compid { get; set; }
            public string ACF_Ipaddress { get; set; }
            public int ACF_Catagary { get; set; }
            public int ACF_yearid { get; set; }
            public bool IsDeleted { get; internal set; }
        }

        //SaveCashFlow(Category2)
        public class CashFlowCategory2
        {
            public int ACF_pkid { get; set; }
            public string ACF_Description { get; set; }
            public int ACF_Custid { get; set; }
            public int ACF_Branchid { get; set; }
            public double ACF_Current_Amount { get; set; }
            public double ACF_Prev_Amount { get; set; }
            public string ACF_Status { get; set; }
            public int ACF_Crby { get; set; }
            public int ACF_Updatedby { get; set; }
            public int ACF_Compid { get; set; }
            public string ACF_Ipaddress { get; set; }
            public int ACF_Catagary { get; set; }
            public int ACF_yearid { get; set; }
            public bool IsDeleted { get; internal set; }
        }

        //SaveCashFlow(Category5)
        public class CashFlowCategory5
        {
            public int ACF_pkid { get; set; }
            public string ACF_Description { get; set; }
            public int ACF_Custid { get; set; }
            public int ACF_Branchid { get; set; }
            public double ACF_Current_Amount { get; set; }
            public double ACF_Prev_Amount { get; set; }
            public string ACF_Status { get; set; }
            public int ACF_Crby { get; set; }
            public int ACF_Updatedby { get; set; }
            public int ACF_Compid { get; set; }
            public string ACF_Ipaddress { get; set; }
            public int ACF_Catagary { get; set; }
            public int ACF_yearid { get; set; }
            public bool IsDeleted { get; internal set; }
        }
    }
}
