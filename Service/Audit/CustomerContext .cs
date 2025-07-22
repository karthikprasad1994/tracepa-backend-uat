using TracePca.Interface.DatabaseConnection;

namespace TracePca.Service.Audit
{
    public class CustomerContext: ICustomerContext
    {
        public string? CustomerCode { get; set; }
    }
}
