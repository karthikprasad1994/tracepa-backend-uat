using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FixedAssetsInterface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetAdditionDashboardController : ControllerBase
    {

        private AssetAdditionDashboardInterface _AssetAdditionDashboardInterface;
        public AssetAdditionDashboardController(AssetAdditionDashboardInterface AssetAdditionDahboardInterface)
        {
            _AssetAdditionDashboardInterface = AssetAdditionDahboardInterface;

        }
    }
}
