using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetTransactionAddition: AssetTransactionAdditionInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;


        public AssetTransactionAddition(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

    }
}
