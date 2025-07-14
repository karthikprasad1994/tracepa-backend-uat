using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;

namespace TracePca.Controllers.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConductAuditController : ControllerBase
    {
        private readonly ConductAuditInterface _conductAuditInterface;
        private readonly EngagementPlanInterface _engagementInterface;

        public ConductAuditController(ConductAuditInterface conductAuditInterface, EngagementPlanInterface engagementInterface)
        {
            _conductAuditInterface = conductAuditInterface;
            _engagementInterface = engagementInterface;
        }

        [HttpGet("LoadAllAuditDDLData")]
        public async Task<IActionResult> LoadAllAuditDDLData(int compId)
        {
            try
            {
                var result = await _conductAuditInterface.LoadAllAuditDDLDataAsync(compId);
                return Ok(new { statusCode = 200, message = "All dropdown data fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("GetCustomerFinancialYear")]
        public async Task<IActionResult> GetCustomerFinancialYear(int compId, int custId)
        {
            try
            {
                var result = await _conductAuditInterface.GetCustomerFinancialYearAsync(compId, custId);
                return Ok(new { statusCode = 200, message = "Customer financial year fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while fetching the customer financial year.", error = ex.Message });
            }
        }

        [HttpGet("LoadAuditNoDDL")]
        public async Task<IActionResult> LoadAuditNoDDL(int compId, int yearId, int custId, int userId)
        {
            try
            {
                var result = await _conductAuditInterface.LoadAuditNoDDLAsync(compId, yearId, custId, userId);
                return Ok(new { statusCode = 200, message = "Audit No dropdown data fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Audit No dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("LoadDRLWithAttachmentsDDL")]
        public async Task<IActionResult> LoadDRLWithAttachmentsDDL(int compId, int auditId)
        {
            try
            {
                var result = await _conductAuditInterface.LoadDRLWithAttachmentsDDLAsync(compId, auditId);
                return Ok(new { statusCode = 200, message = "DRL with attachments loaded successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error occurred while loading DRL with attachments.", error = ex.Message });
            }
        }

        [HttpGet("GetConductAuditWorkPapersByAuditId")]
        public async Task<IActionResult> GetConductAuditWorkPapersByAuditId(int compId, int auditId)
        {
            try
            {
                var result = await _conductAuditInterface.GetConductAuditWorkPapersByAuditIdAsync(compId, auditId);
                return Ok(new { statusCode = 200, message = "Workpapers fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch workpapers.", error = ex.Message });
            }
        }

        [HttpGet("GetConductAuditWorkPaperByWPId")]
        public async Task<IActionResult> GetConductAuditWorkPaperByWPId(int compId, int auditId, int workpaperId)
        {
            try
            {
                var result = await _conductAuditInterface.GetConductAuditWorkPaperByWPIdAsync(compId, auditId, workpaperId);
                return Ok(new { statusCode = 200, message = "Workpaper fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch workpaper.", error = ex.Message });
            }
        }

        [HttpPost("SaveOrUpdateConductAuditWorkpaper")]
        public async Task<IActionResult> SaveOrUpdateConductAuditWorkpaper([FromBody] ConductAuditWorkPaperDetailsDTO dto)
        {
            try
            {
                bool checkDuplicate = await _conductAuditInterface.IsDuplicateWorkpaperRefAsync(dto.SSW_SA_ID ?? 0, dto.SSW_WorkpaperRef, dto.SSW_ID ?? 0);

                if (checkDuplicate)
                {
                    return BadRequest(new { statusCode = 400, message = $"Duplicate workpaper reference found: '{dto.SSW_WorkpaperRef}' already exists." });
                }

                var result = await _conductAuditInterface.SaveOrUpdateConductAuditWorkpaperAsync(dto);
                return Ok(new { statusCode = 200, message = "Conduct Audit Workpaper saved/updated successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to save/update Conduct Audit Workpaper.", error = ex.Message });
            }
        }

        [HttpGet("LoadConductAuditCheckPointHeadings")]
        public async Task<IActionResult> LoadConductAuditCheckPointHeadings(int compId, int auditId)
        {
            try
            {
                var result = await _conductAuditInterface.LoadConductAuditCheckPointHeadingsAsync(compId, auditId);
                return Ok(new { statusCode = 200, message = "CheckPoint headings loaded successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load checkpoint headings.", error = ex.Message });
            }
        }

        [HttpGet("LoadConductAuditCheckPointDetails")]
        public async Task<IActionResult> LoadConductAuditCheckPointDetails(int compId, int auditId, int userId, string? heading)
        {
            try
            {
                var result = await _conductAuditInterface.LoadConductAuditCheckPointDetailsAsync(compId, auditId, userId, heading);
                return Ok(new { statusCode = 200, message = "Audit Checkpoint details loaded successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Audit Checkpoint details.", error = ex.Message });
            }
        }

        [HttpPost("AssignWorkpapersToCheckPoints")]
        public async Task<IActionResult> AssignWorkpapersToCheckPoints([FromBody] List<ConductAuditDetailsDTO> dtos)
        {
            try
            {
                var result = await _conductAuditInterface.AssignWorkpapersToCheckPointsAsync(dtos);
                return Ok(new { statusCode = 200, message = "Workpapers assigned successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to assign workpapers.", error = ex.Message });
            }
        }

        [HttpPost("UpdateConductAuditCheckPointRemarksAndAnnexure")]
        public async Task<IActionResult> UpdateConductAuditCheckPointRemarksAndAnnexure([FromBody] ConductAuditDetailsDTO dto)
        {
            try
            {
                var result = await _conductAuditInterface.UpdateConductAuditCheckPointRemarksAndAnnexureAsync(dto);
                return Ok(new { statusCode = 200, message = "Checkpoint remarks and other details updated successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to update remarks or other details.", error = ex.Message });
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

        [HttpPost("UploadAndSaveCheckPointAttachment")]
        public async Task<IActionResult> UploadAndSaveCheckPointAttachment([FromForm] FileAttachmentDTO dto, int auditId, int checkPointId)
        {
            try
            {
                var (attachmentId, relativeFilePath) = await _conductAuditInterface.UploadAndSaveCheckPointAttachmentAsync(dto, auditId, checkPointId, "StandardAudit");
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

        [HttpPost("GenerateAndDownloadWorkpapersReport")]
        public async Task<IActionResult> GenerateAndDownloadWorkpapersReport(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _conductAuditInterface.GenerateAndDownloadWorkpapersReportAsync(compId, auditId, format);
                return File(fileBytes, contentType, fileName);
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }

        [HttpPost("GenerateAndDownloadCheckPointsReport")]
        public async Task<IActionResult> GenerateAndDownloadCheckPointsReport(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _conductAuditInterface.GenerateAndDownloadCheckPointsReportAsync(compId, auditId, format);
                return File(fileBytes, contentType, fileName);
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }

        [HttpPost("GenerateWorkpapersReportAndGetURLPath")]
        public async Task<IActionResult> GenerateWorkpapersReportAndGetURLPath(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var url = await _conductAuditInterface.GenerateWorkpapersReportAndGetURLPathAsync(compId, auditId, format);
                return Ok(new { statusCode = 200, message = "Conduct Audit Workpaper report generated successfully. Download URL is available.", fileUrl = url });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }

        [HttpPost("GenerateCheckPointsReportAndGetURLPath")]
        public async Task<IActionResult> GenerateCheckPointsReportAndGetURLPath(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var url = await _conductAuditInterface.GenerateCheckPointsReportAndGetURLPathAsync(compId, auditId, format);
                return Ok(new { statusCode = 200, message = "Conduct Audit CheckPoints report generated successfully. Download URL is available.", fileUrl = url });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }

        [HttpGet("LoadUsersByCustomerIdDDL")]
        public async Task<IActionResult> LoadUsersByCustomerIdDDL(int custId)
        {
            try
            {
                var dropdownData = await _conductAuditInterface.LoadUsersByCustomerIdDDLAsync(custId);
                return Ok(new { statusCode = 200, message = "Customer user dropdown data fetched successfully.", data = dropdownData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load Customer user dropdown data.", error = ex.Message });
            }
        }

        [HttpPost("UpdateConductAuditWorkPaperReviewer")]
        public async Task<IActionResult> UpdateConductAuditWorkPaperReviewer(int compID, int auditId, int workpaperId, int userId, string reviewerComments)
        {
            try
            {
                var result = await _conductAuditInterface.UpdateConductAuditWorkPaperReviewerAsync(compID, auditId, workpaperId, userId, reviewerComments);
                return Ok(new { statusCode = 200, message = "Reviewer comments updated successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to update reviewer comments details.", error = ex.Message });
            }
        }

        [HttpGet("GetConductAuditRemarksHistory")]
        public async Task<IActionResult> GetConductAuditRemarksHistory(int compId, int auditId, int conductAuditCheckPointPKId, int checkPointId)
        {
            try
            {
                var result = await _conductAuditInterface.GetConductAuditRemarksHistoryAsync(compId, auditId, conductAuditCheckPointPKId, checkPointId);
                return Ok(new { statusCode = 200, message = "Workpapers fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to fetch remarks history.", error = ex.Message });
            }
        }

        [HttpPost("SaveConductAuditRemarksHistory")]
        public async Task<IActionResult> SaveConductAuditRemarksHistoryAsync([FromBody] ConductAuditRemarksHistoryDTO dto)
        {
            try
            {
                var result = await _conductAuditInterface.SaveConductAuditRemarksHistoryAsync(dto);
                return Ok(new { statusCode = 200, message = "Conduct Audit remarks history saved successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to save Conduct remarks history.", error = ex.Message });
            }
        }
    }
}