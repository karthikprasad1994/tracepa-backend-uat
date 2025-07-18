using Microsoft.AspNetCore.Mvc;

namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleReportDto
    {

        //GetCompanyName
        public class CompanyDetailsDto
        {
            public int Company_ID { get; set; }
            public string Company_Name { get; set; }
        }

        //GetPartners
        public class PartnersDto
        {
            public int usr_Id { get; set; }
            public string Fullname { get; set; }
            public string usr_PhoneNo { get; set; }   
            public string org_name { get; set; }       
        }

        //GetSubHeading
        public class SubHeadingDto
        {
            public int SubheadingID { get; set; }
            public string SubheadingName { get; set; }
        }

        //GetItem
        public class ItemDto
        {
            public int ItemsId { get; set; }
            public string ItemsName { get; set; }
        }

        ////GetSummaryReportForPandL(Income)
        //public class SummaryPnLIncome
        //{      
        //    public int ACID { get; set; }        
        //    public int YearID { get; set; }      
        //    public int CustID { get; set; }      
        //    public int ScheduleTypeID { get; set; }  
        //    public int ChkPt { get; set; }    
        //    public int InAmt { get; set; }       
        //    public int RoundOff { get; set; }        
        //}
        //public class SummaryPnLRowIncome
        //{
        //    public string SrNo { get; set; }
        //    public string HeadingName { get; set; }
        //    public string SubHeadingName { get; set; }
        //    public string HeaderSLNo { get; set; }
        //    public string PrevYearTotal { get; set; }
        //    public string Notes { get; set; }
        //}

        ////GetSummaryReportForPandL(Expenses)
        //public class SummaryPnLExpenses
        //{
        //    public int ACID { get; set; }
        //    public int YearID { get; set; }
        //    public int CustID { get; set; }
        //    public int ScheduleTypeID { get; set; }
        //    public int ChkPt { get; set; }
        //    public int InAmt { get; set; }
        //    public int RoundOff { get; set; }
        //}
        //public class SummaryPnLRowExpenses
        //{
        //    public string SrNo { get; set; }
        //    public string HeadingName { get; set; }
        //    public string SubHeadingName { get; set; }
        //    public string HeaderSLNo { get; set; }
        //    public string PrevYearTotal { get; set; }
        //    public string Notes { get; set; }
        //}

        //GetSummaryReportForPandL(Income and Expenses)
        public class SummaryReportPnL
        {
            public int YearID { get; set; }
            public int CustID { get; set; }
            public int BranchId { get; set; }
           
        }
        public class SummaryReportPnLRow
        {
            public string SrNo { get; set; }
            public string Name { get; set; }
            public string HeaderSLNo { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
        }

        //GetSummaryReportForBalanceSheet(Income and Expenses)
        public class SummaryReportBalanceSheet
        {
            public int YearID { get; set; }
            public int CustID { get; set; }
            public int BranchId { get; set; }

        }
        public class SummaryReportBalanceSheetRow
        {
            public string SrNo { get; set; }
            public string Name { get; set; }
            public string HeaderSLNo { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
        }
    }
}
