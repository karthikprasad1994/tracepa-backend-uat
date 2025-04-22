using TracePca.Data.CustomerRegistration;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;

namespace TracePca.Service
{
    public class AssetAdditionDashboard: AssetAdditionDashboardInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly CustomerRegistrationContext _customerRegistrationContext;

        public AssetAdditionDashboard(Trdmyus1Context dbContext, CustomerRegistrationContext customerDbContext)
        {
            _dbcontext = dbContext;
            _customerRegistrationContext = customerDbContext;

        }
    }
}
