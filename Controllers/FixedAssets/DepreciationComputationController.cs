using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Service.FixedAssetsService;
using static TracePca.Dto.FixedAssets.DepreciationComputationDto;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepreciationComputationController : ControllerBase
    {
        private DepreciationComputationInterface DepreciationComputationInterface;
        private DepreciationComputationInterface _DepreciationComputationService;

        public DepreciationComputationController(AssetCreationInterface AssetCreationInterface)
        {
            DepreciationComputationInterface = DepreciationComputationInterface;
            _DepreciationComputationService = DepreciationComputationInterface;
        }

        //MethodofDepreciation
        [HttpGet("LoadDepreciationCompSLM")]
        public async Task<IActionResult> LoadDepreciationCompSLM(
       [FromQuery] string sNameSpace,
       [FromQuery] int compId,
       [FromQuery] int yearId,
       [FromQuery] int noOfDays,
       [FromQuery] int tNoOfDays,
       [FromQuery] int duration,
       [FromQuery] DateTime startDate,
       [FromQuery] DateTime endDate,
       [FromQuery] int custId,
       [FromQuery] int method)
        {
            try
            {
                // Call service
                List<DepreciationDto> result = await _DepreciationComputationService.LoadDepreciationCompSLMAsync(
                    sNameSpace, compId, yearId, noOfDays, tNoOfDays, duration, startDate, endDate, custId, method
                );

                if (result == null || result.Count == 0)
                    return NotFound(new { Message = "No depreciation records found." });

                return Ok(result); // 200 OK with data
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { Message = argEx.Message }); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Log exception (implement logging)
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message }); // 500 Internal Server Error
            }
        }

    }
}
