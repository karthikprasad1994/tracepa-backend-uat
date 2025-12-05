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

<<<<<<< HEAD
        //
=======
>>>>>>> 2ed7780949550b8113b9d4ee2a732733d8fcb143
        [HttpGet("GetAccountingRatios")]
        public async Task<IActionResult> GetAccountingRatios(int yearId, int customerId, int branchId)
        {
            if (yearId <= 0 || customerId <= 0 || branchId <= 0)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Invalid yearId, customerId, or branchId.",
                    data = (object)null
                });
            }

            try
            {
                var result = await _ScheduleAccountingRatioService.LoadAccRatioAsync(yearId, customerId, branchId);

<<<<<<< HEAD
                if (result == null || result.Ratios == null || !result.Ratios.Any())
=======
                if (result == null || result == null )
>>>>>>> 2ed7780949550b8113b9d4ee2a732733d8fcb143
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No accounting ratios found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Accounting ratios fetched successfully.",
<<<<<<< HEAD
                    data = result.Ratios  // Send DTO list; DataTable can be added if needed
=======
                    data = result  // Send DTO list; DataTable can be added if needed
>>>>>>> 2ed7780949550b8113b9d4ee2a732733d8fcb143
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching accounting ratios.",
                    error = ex.Message
                });
            }
        }
    }
}
