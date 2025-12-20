namespace TracePca.Dto.FixedAssets
{
    public class AssetTransactionAdditionDto
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


    }
}
