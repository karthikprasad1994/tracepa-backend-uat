using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.Audit;

namespace TracePca.Controllers.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditCompletionController : ControllerBase
    {
        private readonly AuditCompletionInterface _auditCompletionInterface;

        public AuditCompletionController(AuditCompletionInterface auditCompletionInterface)
        {
            _auditCompletionInterface = auditCompletionInterface;
        }

        [HttpGet("LoadAllAuditDDLData")]
        public async Task<IActionResult> LoadAllAuditDDLData(int compId)
        {
            var dropdownData = await _auditCompletionInterface.LoadAllAuditDDLDataAsync(compId);
            return Ok(new
            {
                statusCode = 200,
                message = "All Dropdown data fetched successfully.",
                data = dropdownData
            });
        }

        [HttpGet("GetReportTypeDetails")]
        public async Task<IActionResult> GetReportTypeDetails(int compId)
        {
            var result = await _auditCompletionInterface.GetReportTypeDetails(compId);
            if (result == null || !result.Any())
                return NotFound(new { statusCode = 404, message = "No report type details found." });

            return Ok(new
            {
                statusCode = 200,
                message = "Report Type details fetched successfully.",
                data = result
            });
        }

        [HttpGet("LoadAuditNoDDL")]
        public async Task<IActionResult> LoadAuditNoDDL(int compId, int yearId, int custId, int userId)
        {
            var dropdownData = await _auditCompletionInterface.LoadAuditNoDDLAsync(compId, yearId, custId, userId);
            return Ok(new
            {
                statusCode = 200,
                message = "Audit No Dropdown data fetched successfully.",
                data = dropdownData
            });
        }        

        [HttpGet("LoadAuditWorkpaperDDL")]
        public async Task<IActionResult> LoadAuditWorkpaperDDL(int compId, int auditId)
        {
            var dropdownData = await _auditCompletionInterface.LoadAuditWorkpaperDDLAsync(compId, auditId);
            return Ok(new
            {
                statusCode = 200,
                message = "Audit Workpaper No Dropdown data fetched successfully.",
                data = dropdownData
            });
        }
    }
}
