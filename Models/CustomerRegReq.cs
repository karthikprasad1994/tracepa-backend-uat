using TracePca.Models.CustomerRegistration;

namespace TracePca.Models
{
    public class CustomerRegReq
    {
        public SadUserDetail User { get; set; }
        public MmcsCustomerRegistration Customer { get; set; }
    }
}
