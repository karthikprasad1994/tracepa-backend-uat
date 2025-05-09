using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;

namespace TracePca.Controllers.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class EngagementPlanController : ControllerBase
    {
        private readonly EngagementPlanInterface _engagementInterface;

        public EngagementPlanController(EngagementPlanInterface engagementInterface)
        {
            _engagementInterface = engagementInterface;
        }

        [HttpGet("LoadAllDDLData")]
        public async Task<IActionResult> LoadAllDDLData(int compId)
        {
            var dropdownData = await _engagementInterface.LoadAllDDLDataAsync(compId);
            return Ok(new
            {
                statusCode = 200,
                message = "All Dropdown data fetched successfully.",
                data = dropdownData
            });
        }

        [HttpGet("LoadEngagementPlanDDL")]
        public async Task<IActionResult> LoadEngagementPlanDDL(int compId, int yearId, int custId)
        {
            var dropdownData = await _engagementInterface.LoadEngagementPlanDDLAsync(compId, yearId, custId);
            return Ok(new
            {
                statusCode = 200,
                message = "Engagement Plan Dropdown data fetched successfully.",
                data = dropdownData
            });
        }

        [HttpGet("GetReportTypeDetails")]
        public async Task<IActionResult> GetReportTypeDetails(int compId, int reportTypeId)
        {
            var result = await _engagementInterface.GetReportTypeDetails(compId, reportTypeId);
            if (result == null || !result.Any())
                return NotFound(new { statusCode = 404, message = "No report type details found." });

            return Ok(new
            {
                statusCode = 200,
                message = "Report Type details fetched successfully.",
                data = result
            });
        }

        [HttpGet("CheckAndGetEngagementPlanByIds")]
        public async Task<IActionResult> CheckAndGetEngagementPlanByIds(int compId, int customerId, int yearId, int auditTypeId)
        {
            var result = await _engagementInterface.CheckAndGetEngagementPlanByIdsAsync(compId, customerId, yearId, auditTypeId);

            if (result == null)
                return NotFound(new { statusCode = 404, message = "No Engagement Plan details found." });

            return Ok(new
            {
                statusCode = 200,
                message = "Engagement Plan details fetched successfully.",
                data = result
            });
        }

        [HttpGet("GetEngagementPlanById")]
        public async Task<IActionResult> GetEngagementPlanById(int compId, int epPKid)
        {
            var result = await _engagementInterface.GetEngagementPlanByIdAsync(compId, epPKid);

            if (result == null)
                return NotFound(new { statusCode = 404, message = "No Engagement Plan details found." });

            return Ok(new
            {
                statusCode = 200,
                message = "Engagement Plan details fetched successfully.",
                data = result
            });
        }

        [HttpPost("SaveOrUpdateEngagementPlanData")]
        public async Task<IActionResult> SaveOrUpdateEngagementPlanData([FromBody] EngagementDetailsDTO dto)
        {
            var result = await _engagementInterface.SaveOrUpdateEngagementPlanDataAsync(dto);
            if (result > 0)
            {
                if (dto.LOE_Id > 0)
                    return Ok(new { statusCode = 200, message = "Engagement Plan data updated successfully.", Data = result });
                else
                    return Ok(new { statusCode = 200, message = "Engagement Plan data inserted successfully.", Data = result });
            }
            else
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to save Engagement Plan data." });
            }
        }

        [HttpPost("ApproveEngagementPlan")]
        public async Task<IActionResult> ApproveEngagementPlan(int compId, int epPKid, int approvedBy)
        {
            try
            {
                var result = await _engagementInterface.ApproveEngagementPlanAsync(compId, epPKid, approvedBy);
                if (result)
                    return Ok(new { success = true, message = "Engagement Plan approved successfully." });
                else
                    return BadRequest(new { success = false, message = "Engagement Plan approval failed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
