using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetRemarksCount()
        {
            try
            {
                var result = await _dashboardService.GetRemarksSummaryAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Remarks count fetched successfully",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"Internal Server Error: {ex.Message}" });
            }
        }
    }
}
