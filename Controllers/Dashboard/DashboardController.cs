using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using TracePca.Interface.Dashboard;
using static TracePca.Dto.Dashboard.DashboardDto;

namespace TracePca.Controllers.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardInterface _dashboardService;

        public DashboardController(DashboardInterface dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("GetRemarksCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRemarksCount(int yearId, int compId)
        {
            try
            {
                var result = await _dashboardService.GetRemarksSummaryAsync(yearId, compId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Remarks count fetched successfully",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { status = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"Internal Server Error: {ex.Message}" });
            }
        }
        public class ApiResponse<T>
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }



        [HttpGet("PCOB")]
        public async Task<IActionResult> GetStandardAudits(int yearId, int compId)
        {
            try
            {
                var audits = await _dashboardService.GetStandardAuditsAsync(yearId, compId);

                return Ok(new
                {
                    success = true,
                    data = audits
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet("ICAI")]
        public async Task<IActionResult> GetFramework0Audits(int yearId, int compId)
        {

            try
            {
                var audits = await _dashboardService.GetStandardAuditsFramework0Async(yearId, compId);
                return Ok(new { success = true, data = audits });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetLOEProgress")]
        public async Task<IActionResult> GetLOEProgress(int compId, int yearId, int custId = 0)
        {
            try
            {
                var progressData = await _dashboardService.GetLOEProgressAsync(compId, yearId, custId);
                return Ok(new { statusCode = 200, message = "LOE progress data fetched successfully.", data = progressData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load LOE progress data.", error = ex.Message });
            }
        }

        [HttpGet("GetAuditProgress")]
        public async Task<IActionResult> GetAuditProgress(int compId, int yearId, int custId = 0)
        {
            try
            {
                var progressData = await _dashboardService.GetAuditProgressAsync(compId, yearId, custId);
                return Ok(new { statusCode = 200, message = "Audit progress data fetched successfully.", data = progressData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Audit progress data.", error = ex.Message });
            }
        }

        [HttpGet("GetAuditPassedDueDates")]
        public async Task<IActionResult> GetAuditPassedDueDates(int compId, int yearId, int custId = 0)
        {
            try
            {
                var progressData = await _dashboardService.GetAuditPassedDueDatesAsync(compId, yearId, custId);
                return Ok(new { statusCode = 200, message = "Audit Passed due dates data fetched successfully.", data = progressData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Audit Passed due dates data.", error = ex.Message });
            }
        }
    }
}
