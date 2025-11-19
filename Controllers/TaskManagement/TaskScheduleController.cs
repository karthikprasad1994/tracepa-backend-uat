using Microsoft.AspNetCore.Mvc;
using System.Net;
using TracePca.Dto.TaskManagement;
using TracePca.Interface.TaskManagement;

namespace TracePca.Controllers.TaskManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskScheduleController : ControllerBase
    {
        private readonly TaskDashboardInterface _taskDashboardInterface;
        private readonly TaskScheduleInterface _taskScheduleInterface;
        public TaskScheduleController(TaskDashboardInterface taskDashboardInterface, TaskScheduleInterface taskScheduleInterface)
        {
            _taskDashboardInterface = taskDashboardInterface;
            _taskScheduleInterface = taskScheduleInterface;
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


        [HttpGet("loadAllModules")]
        public async Task<IActionResult> loadAllModules(int compId, int projectId)
        {
            try
            {
                var dropdownData = await _taskScheduleInterface.LoadAllModulesAsync(compId, projectId);
                return Ok(new { statusCode = 200, message = "Module/Phase data fetched successfully.", data = dropdownData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Module/Phase data.", error = ex.Message });
            }
        }


        [HttpGet("loadSubTasksByTaskId")]
        public async Task<IActionResult> loadSubTasksByTaskId(int compId, int taskId)
        {
            try
            {
                var dropdownData = await _taskScheduleInterface.loadSubTasksByTaskIdAsync(compId, taskId);
                return Ok(new { statusCode = 200, message = "SubTask(s) data fetched successfully.", data = dropdownData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load SubTask(s) data.", error = ex.Message });
            }
        }


        [HttpPost("SaveOrUpdateTaskSchedule")]
        public async Task<IActionResult> SaveOrUpdateTaskSchedule([FromBody] TaskScheduleCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid request payload." });

            try
            {
                var scheduleId = await _taskScheduleInterface.SaveOrUpdateTaskScheduleAsync(dto);

                return Ok(new { statusCode = 200, message = dto.TMS_Id > 0 ? "Task Schedule updated successfully." : "Task Schedule created successfully.", scheduleId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while saving or updating Task Schedule.", error = ex.Message });
            }
        }


        [HttpGet("GetTaskScheduledDetailsByTaskId")]
        public async Task<IActionResult> GetTaskScheduledDetailsByTaskId([FromQuery] int taskId)
        {
            try
            {
                var data = await _taskScheduleInterface.GetTaskScheduledDetailsByTaskIdAsync(taskId);
                if (data == null)
                    return NotFound(new { statusCode = (int)HttpStatusCode.NotFound, message = "Task not found." });

                return Ok(new { statusCode = 200, message = "Task details fetched successfully.", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch Task details.", error = ex.Message });
            }
        }


        [HttpPost("SaveOrUpdateTaskUpdate")]
        public async Task<IActionResult> SaveOrUpdateTaskUpdate([FromBody] TaskUpdateRequestDto dto)
        {
            if (dto == null)
                return BadRequest(new { statusCode = 400, message = "Invalid request payload." });

            try
            {
                var result = await _taskScheduleInterface.UpdateTaskAndSubtasksAsync(dto);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Task updated successfully.",
                    data = result,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An unexpected error occurred while saving Task update.", error = ex.Message });
            }
        }
    }
}