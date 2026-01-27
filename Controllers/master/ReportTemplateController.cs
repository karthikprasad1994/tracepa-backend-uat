using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Dto.Master;
using TracePca.Interface.Master;
using TracePca.Service.Master;

namespace TracePca.Controllers.Master
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportTemplateController : ControllerBase
    {
        private readonly ReportTemplateInterface _reportTemplateService;

        public ReportTemplateController(ReportTemplateInterface reportTemplateService)
        {
            _reportTemplateService = reportTemplateService;
        }

        [HttpGet("GetReportTypesByFunction")]
        public async Task<IActionResult> GetReportTypesByFunction([FromQuery] int functionId, [FromQuery] int compId)
        {
            var (success, message, data) = await _reportTemplateService.GetReportTypesByFunctionAsync(functionId, compId);
            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetReportContentByReportType")]
        public async Task<IActionResult> GetReportContentByReportType([FromQuery] int reportTypeId, [FromQuery] int compId)
        {
            var (success, message, data) = await _reportTemplateService.GetReportContentByReportTypeAsync(reportTypeId, compId);
            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpPost("SaveOrUpdateReportContent")]
        public async Task<IActionResult> SaveOrUpdateReportContent([FromBody] ReportContentSaveDTO dto)
        {
            try
            {
                var (success, message) = await _reportTemplateService.SaveOrUpdateReportContentAsync(dto);
                return Ok(new { statusCode = 200, success, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving/updating report content: " + ex.Message });
            }
        }

        [HttpPost("SaveReportSortOrder")]
        public async Task<IActionResult> SaveReportSortOrder([FromBody] ReportTemplateSortOrderSaveDTO dto)
        {
            try
            {
                var (success, message) = await _reportTemplateService.SaveOrUpdateReportTemplateSortOrderAsync(dto);
                return Ok(new { statusCode = 200, success, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving/updating report content sort order: " + ex.Message });
            }
        }
    }
}
