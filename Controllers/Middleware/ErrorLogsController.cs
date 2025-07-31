using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.Middleware;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.Middleware
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogsController : ControllerBase
    {
        private readonly ErrorLoggerInterface _loggerService;
        public ErrorLogsController(ErrorLoggerInterface loggerService)
        {
            _loggerService = loggerService;
        }
        // GET: api/<ErrorLogsController>
        [HttpGet("GetErrorLogs")]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _loggerService.GetAllLogsAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "ErrorLogs fetched successfully.",
                data = logs
            });
        }

        [HttpGet("GetErrorLogsById")]
        public async Task<IActionResult> GetLogById(int id)
        {
            var log = await _loggerService.GetLogByIdAsync(id);
            if (log == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = $"Log with ID {id} not found.",
                    data = (object?)null
                });
            }

            return Ok(new
            {
                statusCode = 200,
                message = "ErrorLogs fetched successfully.",
                data = log
            });
        }

    }
}
