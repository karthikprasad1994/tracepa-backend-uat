namespace TracePca.Dto.ClientPortal
{
    public class ClientPortalDto
    {
        public class GetUserCustomerIdRequest
        {
            public string CompanyId { get; set; }

            public string Cust_Name { get; set; }
            public string UserId { get; set; }
        }
        public class UserCustomerResponse
        {
            public string CompanyId { get; set; }
            public string CustomerName { get; set; }
        }


    }
}
