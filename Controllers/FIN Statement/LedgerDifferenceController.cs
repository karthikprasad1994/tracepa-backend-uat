using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerDifferenceController : ControllerBase
    {
        private LedgerDifferenceInterface _LedgerDifferenceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public LedgerDifferenceController(LedgerDifferenceInterface LedgerDifferenceInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _LedgerDifferenceService = LedgerDifferenceInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetHeadWiseDetails
        [HttpGet("GetHeadWiseDetails")]
        public async Task<IActionResult> GetHeadWiseDetails(int compId, int custId, int branchId, int yearId, int durationId)
        {
            try
            {
                var result = await _LedgerDifferenceService.GetHeadWiseDetailsAsync(compId, custId, branchId, yearId, durationId);

                if (result.ScheduleType3 == null && result.ScheduleType4 == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No records found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data fetched successfully.",
                    ScheduleType3 = result.ScheduleType3,
                    ScheduleType4 = result.ScheduleType4
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching data.",
                    Error = ex.Message
                });
            }
        }

        //GetAccountWiseDetails
        [HttpGet("GetAccountWiseDetails")]
        public async Task<IActionResult> GetAccountWiseDetails(
       int compId, int custId, int branchId, int yearId, int durationId)
        {
            try
            {
                var result = await _LedgerDifferenceService.GetAccountWiseDetailsAsync(compId, custId, branchId, yearId, durationId);

                if ((result.ScheduleType3 == null || !result.ScheduleType3.Any()) &&
                    (result.ScheduleType4 == null || !result.ScheduleType4.Any()))
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No records found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data fetched successfully.",
                    ScheduleType3 = result.ScheduleType3,
                    ScheduleType4 = result.ScheduleType4
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching account-wise details.",
                    Error = ex.Message
                });
            }
        }
    }
}
