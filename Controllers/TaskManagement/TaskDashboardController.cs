using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.TaskManagement;
using TracePca.Interface.TaskManagement;

namespace TracePca.Controllers.TaskManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskDashboardController : ControllerBase
    {
        private readonly TaskDashboardInterface _taskDashboardInterface;
        public TaskDashboardController(TaskDashboardInterface taskDashboardInterface)
        {
            _taskDashboardInterface = taskDashboardInterface;
        }

        [HttpGet("LoadAllDDLData")]
        public async Task<IActionResult> LoadAllDDLData(int compId)
        {
            try
            {
                var dropdownData = await _taskDashboardInterface.LoadAllDDLDataAsync(compId);
                return Ok(new { statusCode = 200, message = "All dropdown data fetched successfully.", data = dropdownData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load dropdown data.", error = ex.Message });
            }
        }

        [HttpPost("GetTaskScheduledDashboardData")]
        public async Task<IActionResult> GetTaskScheduledDashboardData([FromBody] TaskDashboardRequestDto dto)
        {
            try
            {
                var data = await _taskDashboardInterface.GetTaskScheduledDashboardDataAsync(dto);
                return Ok(new { statusCode = 200, message = "Task Dashboard data fetched successfully.", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch Task Dashboard data.", error = ex.Message });
            }
        }
    }
}