using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.AssetRegister;
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
        [HttpGet("GetAssetDetailById")]
        public async Task<IActionResult> GetAssetDetailById([FromQuery] int assetId)
        {
            try
            {
                var result = await _AssetRegisterInterface.GetAssetRegDetailsAsync(assetId);

                if (result != null)
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Asset detail fetched successfully.",
                        data = new
                        {
                            assetDetail = new
                            {
                                result.Id,
                                result.CustomerName,
                                result.CustomerId,
                                result.FinancialYear,
                                result.YearId,
                                result.UnitOfMeasurement,
                                result.UnitOfMeasurementId,
                                result.AssetId,
                                result.LocationId,
                              
                                result.AssetClassName,
                                result.AssetCode,
                                result.AssetNo,
                                result.AssetDescription,
                                result.Quantity,
                               
                                result.UsefulLife,
                                putToUseDate = result.PutToUseDate.ToString("yyyy-MM-dd") // ✅ only date string
                            }
                        }
                    });
                }

                return NotFound(new
                {
                    statusCode = 404,
                    message = "Asset detail not found.",
                    data = new { assetDetail = new { } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new { assetDetail = new { } }
                });
            }
        }

        [HttpPut("update/{afamId}")]
        public async Task<IActionResult> UpdateAssetDetails(int afamId,[FromBody] AssetUpdateDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Invalid asset details.");
            }

            try
            {
                // Call service method to update asset details
                await _AssetRegisterInterface.UpdateAssetAsync(afamId, updateDto);

                // Return success response
                return Ok(new { message = "Asset details updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                // _logger.LogError(ex, "Error updating asset details.");

                // Return error response
                return StatusCode(500, new { message = "An error occurred while updating asset details.", error = ex.Message });
            }
        }
    }
}

    

