namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleMastersDto
    {
        //GetCustomersName
        public class CustDto
        {
            public int Cust_Id { get; set; }
            public string? Cust_Name { get; set; }
        }

        //GetDuration
        public class CustDurationDto
        {
            public int Cust_DurtnId { get; set; }
        }

        //GetFinancialYear
        public class FinancialYearDto
        {
            public int YearId { get; set; }
            public string? Id { get; set; }
        }

        //GetBranchName
        public class CustBranchDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

        //GetScheduleHeading
        public class ScheduleHeadingDto
        {
            public int ASH_ID { get; set; }
            public string? ASH_Name { get; set; }
        }

        //GetScheduleSubHeading
        public class ScheduleSubHeadingDto
        {
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
        }

        //GetScheduleItem
        public class ScheduleItemDto
        {
            public int ASI_ID { get; set; }
            public string ASI_Name { get; set; }
        }

        //GetScheduleSubItem
        public class ScheduleSubItemDto
        {
            public int ASSI_ID { get; set; }
            public string ASSI_Name { get; set; }
        }

        //GetCustomerOrgType
        public class OrgTypeDto
        {
            public string OrgType { get; set; }
        }
    }
}
