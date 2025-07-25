using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

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

        //GetSummaryReportForPandL
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

        //GetSummaryReportForBalanceSheet
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
            public string YearId { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
        }

        //GetDetaledReportPandL
        public class DetailedReportPandL
        {
            // Request Parameters
            public int YearID { get; set; }
            public int CustID { get; set; }
            public int BranchID { get; set; }
        }
        public class DetailedReportPandLRow
        {
            internal object HeadingID;

            public string SrNo { get; set; }
            public string Status { get; set; }
            public string Name { get; set; }
            public string HeadingName { get; set; }
            public string SubHeadingName { get; set; }
            public string ItemName { get; set; }
            public string SubItemName { get; set; }
            public string HeaderSLNo { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
            public decimal? CrTotal { get; set; }
            public decimal? DbTotal { get; set; }
            public decimal? CrTotalPrev { get; set; }
            public decimal? DbTotalPrev { get; set; }
        }
    }
}
