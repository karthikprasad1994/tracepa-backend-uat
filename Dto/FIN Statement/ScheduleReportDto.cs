using Microsoft.AspNetCore.Mvc;

namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleReportDto
    {
        private readonly object _ScheduleReportService;

        //GetCustomersName
        public class CustDto
        {
            public int Cust_Id { get; set; }
            public string? Cust_Name { get; set; }
        }

        //GetFinancialYear
        public class FinancialYearDto
        {
            public int YMS_YEARID { get; set; }
            public string? YMS_ID { get; set; }
        }

        //GetBranchName
        public class CustBranchDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

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

        //GetDateFormat
        public class DateFormatSelectionRequestDto
        {
            public int CompanyId { get; set; }
            public string ConfigKey { get; set; }
        }
    }
}
