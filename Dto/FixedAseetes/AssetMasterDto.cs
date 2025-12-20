namespace TracePca.Dto.FixedAssets
{
    public class AssetMasterdto
    {
        //LoadCustomer
        public class CustDto
        {
            public int Cust_Id { get; set; }
            public string Cust_Name { get; set; }
        }

        //LoadStstus
        public class StatusDto
        {
            public string Status { get; set; }
        }


        //FinancialYear
        public class YearDto
        {
            public int YMS_YEARID { get; set; }
            public string YMS_ID { get; set; }
        }

        //Location
        public class LocationDto
        {
            public int LocationId { get; set; }
            public string LocationName { get; set; }
        }

        //LoadDivision
        public class DivisionDto
        {
            public int DivisionId { get; set; }
            public string DivisionName { get; set; }
        }

        //LoadDepartment
        public class DepartmentDto
        {
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
        }
        //LoadBay
        public class BayDto
        {
            public int BayiId { get; set; }
            public string BayiName { get; set; }
        }

        //LoadHeading
        public class HeadingDto
        {
            public int HeadingId { get; set; }
            public string HeadingName { get; set; }
        }


        //LoadSubHeading
        public class SubHeadingDto
        {
            public int SubHeadingId { get; set; }
            public string SubHeadingName { get; set; }
        }

        //AssetClassUnderSubHeading
        public class ItemDto
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
        }
    }
}
