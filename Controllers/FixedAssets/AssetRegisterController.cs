using Microsoft.AspNetCore.Mvc;
using TracePca.Interface;
using TracePca.Interface.FixedAssetsInterface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetRegisterController : ControllerBase
    {
        private AssetRegisterInterface _AssetRegisterInterface;
        public AssetRegisterController(AssetRegisterInterface AssetRegisterInterface)
        {
            _AssetRegisterInterface = AssetRegisterInterface;

        }
        // GET: api/<AssetRegisterController>

        [HttpGet("GetAssetDetails")]
        public async Task<IActionResult> GetAssetDetails([FromQuery] int customerId, [FromQuery] int assetClassId, [FromQuery] int financialYearId)
        {
            try
            {
                var assetDetails = await _AssetRegisterInterface.GetAssetDetailsAsync(customerId, assetClassId, financialYearId);

                if (assetDetails != null && assetDetails.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Asset details fetched successfully.",
                        data = new
                        {
                            assetDetails = assetDetails
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No asset details found for the selected filters.",
                        data = new
                        {
                            assetDetails = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        assetDetails = new List<object>()
                    }
                });
            }
        }


    }
}
