using Microsoft.AspNetCore.Mvc;
using TracePca.Interface;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.FixedAssetsInterface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTransactionAdditionController : ControllerBase
    {

        private AssetTransactionAdditionInterface _AssetTransactionAdditionInterface;
        private AssetTransactionAdditionInterface _AssetTransactionAdditionService;

        public AssetTransactionAdditionController(AssetTransactionAdditionInterface AssetTransactionAdditionInterface)
        {
            _AssetTransactionAdditionInterface = AssetTransactionAdditionInterface;
            _AssetTransactionAdditionService = AssetTransactionAdditionInterface;
        }

        //LoadCustomer
        [HttpGet("GetCustomerNames")]
        public async Task<IActionResult> GetCustomerNames(int CompId)
        {
            try
            {
                var result = await _AssetTransactionAdditionService.LoadCustomerAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No customers found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customers loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching customers.",
                    error = ex.Message
                });
            }
        }

        //LoadStatus
        [HttpGet("LoadStatus")]
        public async Task<IActionResult> LoadStatus(int CompId, string Name)
        {
            try
            {
                var result = await _AssetTransactionAdditionService.LoadStatusAsync(CompId, Name);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No status found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "status loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching status.",
                    error = ex.Message
                });
            }
        }

        //FinancialYear
        [HttpGet("GetYears")]
        public async Task<IActionResult> GetYears(int compId)
        {
            try
            {
                var result = await _AssetTransactionAdditionService.GetYearsAsync(compId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No years found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Years fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching years.",
                    error = ex.Message
                });
            }
        }
    }
}
