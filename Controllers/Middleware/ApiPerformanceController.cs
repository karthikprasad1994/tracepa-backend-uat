using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.Audit;
using TracePca.Interface.Middleware;
using TracePca.Service.Miidleware;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.Middleware
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiPerformanceController : ControllerBase
    {
        private readonly PerformanceInterface _performanceInterface;

        public ApiPerformanceController(PerformanceInterface performanceInterface)
        {
            _performanceInterface = performanceInterface;
        }
    
        // GET: api/<ApiPerformanceController>
        [HttpGet("ApiMetrics")]
        public async Task<IActionResult> CheckPerformance([FromQuery] int lookbackMinutes = 10)
        {
            var summaries = await _performanceInterface.GetFormPerformanceSummariesAsync(lookbackMinutes);
            var alerts = await _performanceInterface.GetSlowApisAsync(lookbackMinutes);

            var result = new
            {
                Summaries = summaries,
                Alerts = alerts
            };

            return Ok(result);

            // GET api/<ApiPerformanceController>/5

           


        }

        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok("Ping OK");
        }

    }

}
