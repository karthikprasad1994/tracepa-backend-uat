using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
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
            try
            {
                var dropdownData = await _auditCompletionInterface.LoadAllAuditDDLDataAsync(compId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "All dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load audit dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("GetReportTypeDetails")]
        public async Task<IActionResult> GetReportTypeDetails(int compId)
        {
            try
            {
                var result = await _auditCompletionInterface.GetReportTypeDetails(compId);
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

        [HttpGet("LoadAuditNoDDL")]
        public async Task<IActionResult> LoadAuditNoDDL(int compId, int yearId, int custId, int userId)
        {
            try
            {
                var dropdownData = await _auditCompletionInterface.LoadAuditNoDDLAsync(compId, yearId, custId, userId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Audit number dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load audit number dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("LoadAuditWorkPaperDDL")]
        public async Task<IActionResult> LoadAuditWorkPaperDDL(int compId, int auditId)
        {
            try
            {
                var dropdownData = await _auditCompletionInterface.LoadAuditWorkPaperDDLAsync(compId, auditId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Audit workpaper dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load audit workpaper dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("GetAuditCompletionDetailsById")]
        public async Task<IActionResult> GetAuditCompletionDetailsById(int compId, int auditId)
        {
            try
            {
                var dropdownData = await _auditCompletionInterface.GetAuditCompletionDetailsByIdAsync(compId, auditId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Audit completion details fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch audit completion details.", error = ex.Message });
            }
        }

        [HttpGet("GetAuditCompletionSubPointDetails")]
        public async Task<IActionResult> GetAuditCompletionSubPointDetails(int compId, int auditId, int checkPointId)
        {
            try
            {
                var dropdownData = await _auditCompletionInterface.GetAuditCompletionSubPointDetailsAsync(compId, auditId, checkPointId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Subpoint details for selected checkpoint fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch subpoint details.", error = ex.Message });
            }
        }

        [HttpPost("SaveOrUpdateAuditCompletionData")]
        public async Task<IActionResult> SaveOrUpdateAuditCompletionData([FromBody] AuditCompletionDTO dto)
        {
            try
            {
                var result = await _auditCompletionInterface.SaveOrUpdateAuditCompletionDataAsync(dto);
                if (result > 0)
                {
                    if (dto.SAC_AuditID > 0)
                        return Ok(new { statusCode = 200, message = "Audit completion data updated successfully.", Data = result });
                    else
                        return Ok(new { statusCode = 200, message = "Audit completion data inserted successfully.", Data = result });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "No audit completion data was saved." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating audit completion data.", error = ex.Message });
            }
        }

        [HttpPost("UpdateSignedByUDINInAudit")]
        public async Task<IActionResult> UpdateSignedByUDINInAudit([FromBody] AuditSignedByUDINRequestDTO dto)
        {
            try
            {
                var result = await _auditCompletionInterface.UpdateSignedByUDINInAuditAsync(dto);

                if (result > 0)
                {
                    return Ok(new { statusCode = 200, message = "Audit Status, SignedBy and UDIN details updated successfully.", Data = result });
                }
                else
                {
                    return BadRequest(new { statusCode = 400, message = "No audit completion data was saved." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating audit completion data.", error = ex.Message });
            }
        }

        [HttpPost("GenerateAndDownloadReport")]
        public async Task<IActionResult> GenerateAndDownloadReport(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _auditCompletionInterface.GenerateAndDownloadReportAsync(compId, auditId, format);

                return File(fileBytes, contentType, fileName);
            }
            catch
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Failed to generate report."
                });
            }
        }
    }
}
