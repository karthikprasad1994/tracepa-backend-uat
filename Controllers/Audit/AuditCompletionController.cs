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
        private readonly EngagementPlanInterface _engagementInterface;

        public AuditCompletionController(AuditCompletionInterface auditCompletionInterface, EngagementPlanInterface engagementInterface)
        {
            _auditCompletionInterface = auditCompletionInterface;
            _engagementInterface = engagementInterface;
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

        [HttpGet("GetAuditClosureSubPointDetails")]
        public async Task<IActionResult> GetAuditClosureSubPointDetails(int compId, int auditId, int checkPointId)
        {
            try
            {
                var dropdownData = await _auditCompletionInterface.GetAuditClosureSubPointDetailsAsync(compId, auditId, checkPointId);
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

        [HttpPost("SaveOrUpdateAuditCompletionSubPointData")]
        public async Task<IActionResult> SaveOrUpdateAuditCompletionSubPointData([FromBody] AuditCompletionSingleDTO dto)
        {
            try
            {
                var result = await _auditCompletionInterface.SaveOrUpdateAuditCompletionSubPointDataAsync(dto);
                if (result > 0)
                {
                    return Ok(new { statusCode = 200, message = "Audit completion subpoint data updated successfully.", Data = result });                       
                }
                else
                {
                    return Ok(new { statusCode = 200, message = "Audit completion subpoint data inserted successfully.", Data = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating audit completion subpoint data.", error = ex.Message });
            }
        }

        [HttpPost("SaveOrUpdateAuditCompletionData")]
        public async Task<IActionResult> SaveOrUpdateAuditCompletionData([FromBody] AuditCompletionDTO dto)
        {
            try
            {
                var result = await _auditCompletionInterface.SaveOrUpdateAuditCompletionDataAsync(dto);
                if (result > 0)
                    return Ok(new { statusCode = 200, message = "Audit completion data updated successfully.", Data = result });
                else
                    return Ok(new { statusCode = 200, message = "Audit completion data inserted successfully.", Data = result });
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

        [HttpGet("GetSignedByUDINInAuditAsync")]
        public async Task<IActionResult> GetSignedByUDINInAudit(int compId, int auditId)
        {
            try
            {
                var data = await _auditCompletionInterface.GetSignedByUDINInAuditAsync(compId, auditId);

                if (data == null || (data.SA_SignedBy == 0 && string.IsNullOrWhiteSpace(data.SA_UDIN)))
                {
                    return Ok(new { statusCode = 200, message = "No Data." });
                }

                return Ok(new { statusCode = 200, message = "Audit completion SignedByUDIN details fetched successfully.", data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch audit completion SignedByUDIN details.", error = ex.Message });
            }
        }

        [HttpGet("LoadAllAttachmentsById")]
        public async Task<IActionResult> LoadAllAttachmentsByIdAsync(int compId, int attachId)
        {
            try
            {
                var result = await _engagementInterface.LoadAllAttachmentsByIdAsync(compId, attachId);
                return Ok(new { success = true, message = result.Count > 0 ? "Attachments loaded successfully." : "No attachments found.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while loading attachments.", error = ex.Message });
            }
        }

        [HttpPost("UploadAndSaveAttachment")]
        public async Task<IActionResult> UploadAndSaveAttachment([FromForm] FileAttachmentDTO dto)
        {
            try
            {
                var (attachmentId, relativeFilePath) = await _engagementInterface.UploadAndSaveAttachmentAsync(dto, "StandardAudit");
                if (attachmentId > 0)
                {
                    return Ok(new { success = true, message = "File uploaded and saved successfully.", data = attachmentId });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "File upload failed. No attachment record was saved." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while uploading the file: {ex.Message}" });
            }
        }

        [HttpPost("RemoveAttachmentDoc")]
        public async Task<IActionResult> RemoveAttachmentDoc(int compId, int attachId, int docId, int userId)
        {
            try
            {
                await _engagementInterface.RemoveAttachmentDocAsync(compId, attachId, docId, userId);
                return Ok(new { success = true, message = "Attachment marked as deleted successfully.", data = docId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while marking the attachment as deleted: {ex.Message}" });
            }
        }

        [HttpPost("UpdateAttachmentDocDescription")]
        public async Task<IActionResult> UpdateAttachmentDocDescription(int compId, int attachId, int docId, int userId, string description)
        {
            try
            {
                await _engagementInterface.UpdateAttachmentDocDescriptionAsync(compId, attachId, docId, userId, description);
                return Ok(new { success = true, message = "Attachment description updated successfully.", data = docId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while updating the attachment description: {ex.Message}" });
            }
        }

        [HttpGet("DownloadAttachment")]
        public async Task<IActionResult> DownloadAttachment(int compId, int attachId, int docId)
        {
            try
            {
                var (isFileExists, messageOrfileUrl) = await _engagementInterface.GetAttachmentDocDetailsByIdAsync(compId, attachId, docId, "StandardAudit");
                if (isFileExists)
                {
                    return Ok(new { statusCode = 200, success = true, fileUrl = messageOrfileUrl });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = messageOrfileUrl });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while downloading the file: {ex.Message}" });
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

        [HttpPost("GenerateReportAndGetURLPath")]
        public async Task<IActionResult> GenerateReportAndGetURLPath(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var url = await _auditCompletionInterface.GenerateReportAndGetURLPathAsync(compId, auditId, format);
                return Ok(new { statusCode = 200, message = "Audit Completion report generated successfully. Download URL is available.", fileUrl = url });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }

        [HttpPost("GenerateACSubPointsReportAndGetURLPath")]
        public async Task<IActionResult> GenerateACSubPointsReportAndGetURLPath(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var url = await _auditCompletionInterface.GenerateReportAndGetURLPathAsync(compId, auditId, format);
                return Ok(new { statusCode = 200, message = "Audit Completion sub points report generated successfully. Download URL is available.", fileUrl = url });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }

        [HttpGet("LoadAllAuditAttachmentsByAuditId")]
        public async Task<IActionResult> LoadAllAuditAttachmentsByAuditId(int compId, int auditId)
        {
            try
            {
                var result = await _auditCompletionInterface.LoadAllAuditAttachmentsByAuditIdAsync(compId, auditId);
                var isAnyDataPresent = result.BeginningAuditAttachments?.Count > 0 || result.DuringAuditAttachments?.Count > 0 || result.NearingEndAuditAttachments?.Count > 0 || result.WorkpaperAttachments?.Count > 0 || result.ConductAuditAttachments?.Count > 0;
                return Ok(new { success = true, message = isAnyDataPresent ? "Attachments loaded successfully." : "No attachments found.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while loading attachments.", error = ex.Message });
            }
        }

        [HttpGet("DownloadAllAuditAttachmentsByAuditId")]
        public async Task<IActionResult> DownloadAllAuditAttachmentsByAuditId(int compId, int auditId, int userId, string ipAddress)
        {
            try
            {
                var (isFileExists, messageOrfileUrl) = await _auditCompletionInterface.DownloadAllAuditAttachmentsByAuditIdAsync(compId, auditId, userId, ipAddress);
                if (isFileExists)
                {
                    return Ok(new { statusCode = 200, success = true, fileUrl = messageOrfileUrl });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = messageOrfileUrl });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while downloading the file: {ex.Message}" });
            }
        }
    }
}
