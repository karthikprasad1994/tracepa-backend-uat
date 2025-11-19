using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.TaskManagement;
using TracePca.Interface.TaskManagement;

namespace TracePca.Controllers.TaskManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskInvoiceAndReportController : ControllerBase
    {
        private readonly TaskInvoiceAndReportInterface _taskInvoiceAndReportInterface;
        public TaskInvoiceAndReportController(TaskInvoiceAndReportInterface taskInvoiceAndReportInterface)
        {
            _taskInvoiceAndReportInterface = taskInvoiceAndReportInterface;
        }

        [HttpGet("LoadAllDDLData")]
        public async Task<IActionResult> LoadAllDDLData(int compId)
        {
            try
            {
                var dropdownData = await _taskInvoiceAndReportInterface.LoadAllDDLDataAsync(compId);
                return Ok(new { statusCode = 200, message = "All dropdown data fetched successfully.", data = dropdownData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("GetCompanyInvoiceDetails")]
        public async Task<IActionResult> GetCompanyInvoiceDetails(int compId, int CompanyId)
        {
            try
            {
                var dropdownData = await _taskInvoiceAndReportInterface.GetCompanyInvoiceDetailsAsync(compId, CompanyId);
                return Ok(new { statusCode = 200, message = "Company data fetched successfully.", data = dropdownData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Company data.", error = ex.Message });
            }
        }
    }
}