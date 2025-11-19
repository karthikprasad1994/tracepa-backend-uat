using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgingAnalysisController : ControllerBase
    {
        private AgingAnalysisInterface _AgingAnalysisInterface;
        private AgingAnalysisInterface _AgingAnalysisService;
        private readonly IWebHostEnvironment _env;

        public AgingAnalysisController(AgingAnalysisInterface AgingAnalysisInterface, IWebHostEnvironment env)
        {
            _AgingAnalysisInterface = AgingAnalysisInterface;
            _AgingAnalysisService = AgingAnalysisInterface;
            _env = env;
        }

        //GetAnalysisBasedOnMonthForTradePayables
        [HttpGet("GetAnalysisBasedOnMonthForTradePayables")]
        public async Task<IActionResult> GetTradePayables([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId,
        [FromQuery] int YearId)
        {
            try
            {
                var result = await _AgingAnalysisService.GetTradePayablesAsync(
                    CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No records found for the given period and parameters.",
                        data = (object)null
                    });
                }
                return Ok(new
                {
                    statusCode = 200,
                    message = "Analysis data loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching analysis data based on month.",
                    error = ex.Message
                });
            }
        }

        //GetAnalysisBasedOnMonthForTradeReceivables
        [HttpGet("GetAnalysisBasedOnMonthForTradeReceivables")]
        public async Task<IActionResult> GetTradeReceiveables([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId,
       [FromQuery] int YearId)
        {
            try
            {
                var result = await _AgingAnalysisService.GetTradeReceiveablesAsync(
                    CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No records found for the given period and parameters.",
                        data = (object)null
                    });
                }
                return Ok(new
                {
                    statusCode = 200,
                    message = "Analysis data loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching analysis data based on month.",
                    error = ex.Message
                });
            }
        }

        //GetAnalysisBasedOnMonthForTradePayablesById
        [HttpGet("GetAnalysisBasedOnMonthForTradePayablesById")]
        public async Task<IActionResult> GetTradePayablesById([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId,
        [FromQuery] int YearId)
        {
            try
            {
                var result = await _AgingAnalysisService.GetTradePayablesByIdAsync(
                    CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No records found for the given period and parameters.",
                        data = (object)null
                    });
                }
                return Ok(new
                {
                    statusCode = 200,
                    message = "Analysis data loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching analysis data based on month.",
                    error = ex.Message
                });
            }
        }

       // //GetAnalysisBasedOnMonthForTradeReceivablesById
       // [HttpGet("GetAnalysisBasedOnMonthForTradeReceivablesById")]
       // public async Task<IActionResult> GetTradeReceiveablesById([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId,
       //[FromQuery] int YearId)
       // {
       //     try
       //     {
       //         var result = await _AgingAnalysisService.GetTradeReceiveablesByIdAsync(
       //             CompId, CustId, BranchId, YearId);

       //         if (result == null || !result.Any())
       //         {
       //         {
       //             return NotFound(new
       //             {
       //                 statusCode = 404,
       //                 message = "No records found for the given period and parameters.",
       //                 data = (object)null
       //             });
       //         }
       //         return Ok(new
       //         {
       //             statusCode = 200,
       //             message = "Analysis data loaded successfully.",
       //             data = result
       //         });
       //     }

       //     catch (Exception ex)
       //     {
       //         return StatusCode(500, new
       //         {
       //             statusCode = 500,
       //             message = "An error occurred while fetching analysis data based on month.",
       //             error = ex.Message
       //         });
       //     }
        //}
    }
}
