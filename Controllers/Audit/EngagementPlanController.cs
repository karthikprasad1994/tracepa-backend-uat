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
            try
            {
                var dropdownData = await _engagementInterface.LoadAllDDLDataAsync(compId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "All dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("LoadEngagementPlanDDL")]
        public async Task<IActionResult> LoadEngagementPlanDDL(int compId, int yearId, int custId)
        {
            try
            {
                var dropdownData = await _engagementInterface.LoadEngagementPlanDDLAsync(compId, yearId, custId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Engagement Plan dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Engagement Plan dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("GetReportTypeDetails")]
        public async Task<IActionResult> GetReportTypeDetails(int compId, int reportTypeId)
        {
            try
            {
                var result = await _engagementInterface.GetReportTypeDetails(compId, reportTypeId);
                if (result == null || !result.Any())
                    return NotFound(new { statusCode = 404, message = "No report type details found." });

                return Ok(new
                {
                    statusCode = 200,
                    message = "Report type details fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch report type details.", error = ex.Message });
            }
        }

        [HttpGet("CheckAndGetEngagementPlanByIds")]
        public async Task<IActionResult> CheckAndGetEngagementPlanByIds(int compId, int customerId, int yearId, int auditTypeId)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch Engagement Plan data.", error = ex.Message });
            }
        }

        [HttpGet("GetEngagementPlanById")]
        public async Task<IActionResult> GetEngagementPlanById(int compId, int epPKid)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to retrieve Engagement Plan details.", error = ex.Message });
            }
        }

        [HttpPost("SaveOrUpdateEngagementPlanData")]
        public async Task<IActionResult> SaveOrUpdateEngagementPlanData([FromBody] EngagementPlanDetailsDTO dto)
        {
            try
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
                    return StatusCode(500, new { statusCode = 500, message = "No Engagement Plan data was saved." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating Engagement Plan data.", error = ex.Message });
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
                return StatusCode(500, new { success = false, message = "An error occurred while approving the Engagement Plan.", error = ex.Message });
            }
        }
    }
}
