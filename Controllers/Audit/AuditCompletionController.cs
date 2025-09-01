using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("GetReportTypeDetailsByAuditId")]
        public async Task<IActionResult> GetReportTypeDetailsByAuditId(int compId, int auditId)
        {
            try
            {
                var result = await _auditCompletionInterface.GetReportTypeDetailsByAuditId(compId, auditId);
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
                int result = await _auditCompletionInterface.CheckCAEIndependentAuditorsReportSavedAsync(dto.SA_CompID, dto.SA_ID);

                if (result == 0)
                {
                    return BadRequest(new { statusCode = 400, message = "Report of Independent Registered Public Accounting Firm details have not been generated for this audit. Please generate the report before saving Audit Completion data." });
                }
                else if (result == 1)
                {
                    return BadRequest(new { statusCode = 400, message = "Please complete all mandatory checkpoints before saving Audit Completion." });
                }
                else if (result == 2)
                {
                    var res = await _auditCompletionInterface.UpdateSignedByUDINInAuditAsync(dto);
                    if (res > 0)
                    {
                        return Ok(new { statusCode = 200, message = "Audit Status, SignedBy and UDIN details updated successfully.", Data = res });
                    }
                    else
                    {
                        return BadRequest(new { statusCode = 400, message = "No Audit Completion data was saved." });
                    }
                }
                else if (result == 3)
                {
                    return Ok(new { statusCode = 200, message = "No checkpoints exist for this audit." });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "Unexpected result from CAE status check."});
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating Audit Completion data.", error = ex.Message });
            }
        }

        [HttpPost("UpdateAuditCompletionStatus")]
        public async Task<IActionResult> UpdateAuditCompletionStatus(int compId, int auditId)
        {
            try
            {
                int result = await _auditCompletionInterface.CheckCAEIndependentAuditorsReportSavedAsync(compId, auditId);

                if (result == 0)
                {
                    return BadRequest(new { statusCode = 400, message = "Report of Independent Registered Public Accounting Firm details have not been generated for this audit. Please generate the report before saving Audit Completion data." });
                }
                else if (result == 1)
                {
                    return BadRequest(new { statusCode = 400, message = "Please complete all mandatory checkpoints before saving Audit Completion." });
                }
                else if (result == 2)
                {
                    var res = await _auditCompletionInterface.UpdateAuditCompletionStatusAsync(compId, auditId);
                    if (res > 0)
                    {
                        return Ok(new { statusCode = 200, message = "Audit Completion Status details updated successfully.", Data = res });
                    }
                    else
                    {
                        return BadRequest(new { statusCode = 400, message = "No Audit Completion data was saved." });
                    }
                }
                else if (result == 3)
                {
                    return Ok(new { statusCode = 200, message = "No checkpoints exist for this audit." });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "Unexpected result from CAE status check." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating Audit Completion Status.", error = ex.Message });
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

                return Ok(new { statusCode = 200, message = "Audit Completion SignedByUDIN details fetched successfully.", data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch Audit Completion SignedByUDIN details.", error = ex.Message });
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
                int result = await _auditCompletionInterface.CheckCAEIndependentAuditorsReportSavedAsync(compId, auditId);

                if (result == 0)
                {
                    return BadRequest(new { statusCode = 400, message = "Report of Independent Registered Public Accounting Firm details have not been generated for this audit. Please generate the report before saving Audit Completion data." });
                }
                else if (result == 1)
                {
                    return BadRequest(new { statusCode = 400, message = "Please complete all mandatory checkpoints before saving Audit Completion." });
                }
                else if (result == 2)
                {
                    var (fileBytes, contentType, fileName) = await _auditCompletionInterface.GenerateAndDownloadReportAsync(compId, auditId, format);
                    return File(fileBytes, contentType, fileName);
                }
                else if (result == 3)
                {
                    return Ok(new { statusCode = 200, message = "No checkpoints exist for this audit." });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "Unexpected result from CAE status check." });
                }                
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
                int result = await _auditCompletionInterface.CheckCAEIndependentAuditorsReportSavedAsync(compId, auditId);

                if (result == 0)
                {
                    return BadRequest(new { statusCode = 400, message = "Report of Independent Registered Public Accounting Firm details have not been generated for this audit. Please generate the report before saving Audit Completion data." });
                }
                else if (result == 1)
                {
                    return BadRequest(new { statusCode = 400, message = "Please complete all mandatory checkpoints before saving Audit Completion." });
                }
                else if (result == 2)
                {
                    var url = await _auditCompletionInterface.GenerateReportAndGetURLPathAsync(compId, auditId, format);
                    return Ok(new { statusCode = 200, message = "Audit Completion report generated successfully. Download URL is available.", fileUrl = url });
                }
                else if (result == 3)
                {
                    return Ok(new { statusCode = 200, message = "No checkpoints exist for this audit." });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "Unexpected result from CAE status check." });
                }
                
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }

        [HttpPost("GenerateACSubPointsReportAndGetURLPath")]
        public async Task<IActionResult> GenerateACSubPointsReportAndGetURLPath(int compId, int auditId, int userId, string format = "pdf")
        {
            try
            {
                var url = await _auditCompletionInterface.GenerateACSubPointsReportAndGetURLPathAsync(compId, auditId, userId, format);
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
                var customData = new Dictionary<string, object>
                    {
                        { "Audit Plan", result?.AuditPlanAttachments ?? new List<AttachmentGroupDTO>() },
                        { "Pre / Post Audit", result?.BeginningNearEndAuditAttachments ?? new List<AttachmentGroupDTO>() },
                        { "During Audit", result?.DuringAuditAttachments ?? new List<AttachmentGroupDTO>() },
                        { "Workpaper", result?.WorkpaperAttachments ?? new List<AttachmentGroupDTO>() },
                        { "Conduct Audit Checkpoints", result?.ConductAuditAttachments ?? new List<AttachmentGroupDTO>() }
                    };

                var isAnyDataPresent = customData.Values.OfType<IEnumerable<object>>().Any(list => list.Any());
                return Ok(new { success = true, message = isAnyDataPresent ? "Attachments loaded successfully." : "No attachments found.", data = customData });
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

        [HttpPost("UpdateArchiveInAudit")]
        public async Task<IActionResult> UpdateArchiveInAudit([FromBody] AuditArchiveDTO dto)
        {
            try
            {
                int result = await _auditCompletionInterface.CheckCAEIndependentAuditorsReportSavedAsync(dto.SA_CompID, dto.SA_ID);

                if (result == 0)
                {
                    return BadRequest(new { statusCode = 400, message = "Report of Independent Registered Public Accounting Firm details have not been generated for this audit. Please generate the report before saving Audit Completion data." });
                }
                else if (result == 1)
                {
                    return BadRequest(new { statusCode = 400, message = "Please complete all mandatory checkpoints before saving Audit Completion." });
                }
                else if (result == 2)
                {
                    var res = await _auditCompletionInterface.UpdateArchiveInAuditAsync(dto);
                    if (res > 0)
                    {
                        return Ok(new { statusCode = 200, message = "Audit Archive details updated successfully.", Data = res });
                    }
                    else
                    {
                        return BadRequest(new { statusCode = 400, message = "No Audit Archive data was saved." });
                    }
                }
                else if (result == 3)
                {
                    return Ok(new { statusCode = 200, message = "No checkpoints exist for this audit." });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "Unexpected result from CAE status check." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating Audit Archive data.", error = ex.Message });
            }
        }
    }
}
