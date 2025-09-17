using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleAccoutingRatioController : ControllerBase
    {

        private ScheduleAccountingRatioInterface _ScheduleAccountingRatioService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public ScheduleAccoutingRatioController(Trdmyus1Context dbcontext, IConfiguration configuration, ScheduleAccountingRatioInterface ScheduleAccountingRatioInterface, IHttpContextAccessor httpContextAccessor)
        {

            _ScheduleAccountingRatioService = ScheduleAccountingRatioInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;

        }

        //GetAccoutingRatio
        [HttpGet("GetRatio")]
        public async Task<IActionResult> GetRatio([FromQuery] int yearId, [FromQuery] int customerId)
        {
            if (yearId <= 0 || customerId <= 0)
                return BadRequest(new { Success = false, Message = "Invalid yearId or customerId." });

            try
            {
                var result = await _ScheduleAccountingRatioService.GetAccountingRatioAsync(yearId, customerId);
                return Ok(new
                {
                    Success = true,
                    Message = "Accounting ratio fetched successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "An internal error occurred. Please contact support.",
                    Error = ex.Message
                });
            }
        }

        //GetAccoutingRatio2
        [HttpGet("GetBorrowingsVsShareholders")]
        public async Task<IActionResult> GetBorrowingsVsShareholders([FromQuery] int yearId, [FromQuery] int customerId, [FromQuery] int branchId)
        {
            if (yearId <= 0 || customerId <= 0 || branchId <= 0)
                return BadRequest("Invalid yearId, customerId, or branchId.");

            try
            {
                var result = await _ScheduleAccountingRatioService.GetBorrowingsVsShareholdersAsync(yearId, customerId, branchId);

                return Ok(new
                {
                    Success = true,
                    Message = "Borrowings vs Shareholders Funds ratio calculated successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Error calculating Borrowings vs Shareholders Funds ratio",
                    Error = ex.Message
                });
            }
        }
    }
}
