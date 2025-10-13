namespace TracePca.Dto.FIN_Statement
{
    public class CashFlowDto
    {

        ////SaveCashFlow(Category 1)
        //public class CashFlowCategory1
        //{
        //    public int ACF_pkid { get; set; }
        //    public string ACF_Description { get; set; }
        //    public int ACF_Custid { get; set; }
        //    public int ACF_Branchid { get; set; }
        //    public double ACF_Current_Amount { get; set; }
        //    public double ACF_Prev_Amount { get; set; }
        //    public string ACF_Status { get; set; }
        //    public int ACF_Crby { get; set; }
        //    public int ACF_Updatedby { get; set; }
        //    public int ACF_Compid { get; set; }
        //    public string ACF_Ipaddress { get; set; }
        //    public int ACF_Catagary { get; set; }
        //    public int ACF_Yearid { get; set; }
        //}

        ////SaveCashFlow(Category 2)
        //public class CashFlowCategory2
        //{
        //    public int ACF_pkid { get; set; }
        //    public string ACF_Description { get; set; }
        //    public int ACF_Custid { get; set; }
        //    public int ACF_Branchid { get; set; }
        //    public double ACF_Current_Amount { get; set; }
        //    public double ACF_Prev_Amount { get; set; }
        //    public string ACF_Status { get; set; }
        //    public int ACF_Crby { get; set; }
        //    public int ACF_Updatedby { get; set; }
        //    public int ACF_Compid { get; set; }
        //    public string ACF_Ipaddress { get; set; }
        //    public int ACF_Catagary { get; set; }
        //    public int ACF_yearid { get; set; }
        //}


        ////SaveCashFlow(Category 3)
        //public class CashFlowCategory3
        //{
        //    public int ACF_pkid { get; set; }
        //    public string ACF_Description { get; set; }
        //    public int ACF_Custid { get; set; }
        //    public int ACF_Branchid { get; set; }
        //    public double ACF_Current_Amount { get; set; }
        //    public double ACF_Prev_Amount { get; set; }
        //    public string ACF_Status { get; set; }
        //    public int ACF_Crby { get; set; }
        //    public int ACF_Updatedby { get; set; }
        //    public int ACF_Compid { get; set; }
        //    public string ACF_Ipaddress { get; set; }
        //    public int ACF_Catagary { get; set; }
        //    public int ACF_Yearid { get; set; }
        //}

        ////SaveCashFlow(Category 4)
        //public class CashFlowCategory4
        //{
        //    public int ACF_pkid { get; set; }
        //    public string ACF_Description { get; set; }
        //    public int ACF_Custid { get; set; }
        //    public int ACF_Branchid { get; set; }
        //    public double ACF_Current_Amount { get; set; }
        //    public double ACF_Prev_Amount { get; set; }
        //    public string ACF_Status { get; set; }
        //    public int ACF_Crby { get; set; }
        //    public int ACF_Updatedby { get; set; }
        //    public int ACF_Compid { get; set; }
        //    public string ACF_Ipaddress { get; set; }
        //    public int ACF_Catagary { get; set; }
        //    public int ACF_Yearid { get; set; }
        //}

        ////SaveCashFlow(Category 5)
        //public class CashFlowCategory5
        //{
        //    public int ACF_pkid { get; set; }
        //    public string ACF_Description { get; set; }
        //    public int ACF_Custid { get; set; }
        //    public int ACF_Branchid { get; set; }
        //    public double ACF_Current_Amount { get; set; }
        //    public double ACF_Prev_Amount { get; set; }
        //    public string ACF_Status { get; set; }
        //    public int ACF_Crby { get; set; }
        //    public int ACF_Updatedby { get; set; }
        //    public int ACF_Compid { get; set; }
        //    public string ACF_Ipaddress { get; set; }
        //    public int ACF_Catagary { get; set; }
        //    public int ACF_yearid { get; set; }
        //}

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
    }
}
