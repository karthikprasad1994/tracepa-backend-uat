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

            public string status { get; set; }
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
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
            public string status { get; set; }
        }

        //GetDetaledReportPandL
        public class DetailedReportParams
        {
            public int CustID { get; set; }
            public int ScheduleTypeID { get; set; }
            public int InAmt { get; set; }
        }
        public class DetailedReportRow
        {
            public string Status { get; set; }  
            public string Name { get; set; }   
        }


        public class ScheduleReportRequestDto
        {
            public int CustomerId { get; set; }
            public int PartnerId { get; set; }
            public int CompanyId { get; set; }
        }
        public class ScheduleReportResponseDto
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerAddress { get; set; }
            public string CINNumber { get; set; }
            public List<PartnerDto> Partners { get; set; } = new();
        }

        public class PartnerDto
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string PhoneNo { get; set; }
            public string OrgName { get; set; }
        }
        // Dtos/OrgTypeRequestDto.cs
        public class OrgTypeRequestDto
        {
            public int CustomerId { get; set; }
            public int CompanyId { get; set; }
        }

        // Dtos/PersonDto.cs
        public class PersonDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DIN { get; set; }

        }

        // Dtos/OrgTypeResponseDto.cs
        public class OrgTypeResponseDto
        {
            public string OrgType { get; set; }
            public string Label { get; set; } // "Partners" or "Directors"
            public List<PersonDto> Persons { get; set; }
        }
        public class CompanyDto
        {
            public int Company_ID { get; set; }
            public string Company_Name { get; set; }
        }

    }
}
