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
                var result = await _engagementInterface.UploadAndSaveAttachmentAsync(dto);
                if (result > 0)
                {
                    return Ok(new { success = true, message = "File uploaded and saved successfully.", data = result });
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
                var result = await _engagementInterface.GetAttachmentDocDetailsByIdAsync(compId, attachId, docId);
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Attachment not found." });
                }

                string fileExt = result.ATCH_EXT;
                if (string.IsNullOrEmpty(fileExt))
                {
                    return NotFound(new { success = false, message = "File extension not found." });
                }

                string relativeFolder = Path.Combine("Uploads", "Audit", (docId / 301).ToString());
                string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

                string fileName = $"{docId}.{fileExt}";
                string filePath = Path.Combine(absoluteFolder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { success = false, message = "File not found." });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while downloading the file: {ex.Message}" });
            }
        }


        [HttpPost("Generatepdf/word")]
        public async Task<IActionResult> Generate([FromBody] EngagementPlanDetailsDTO data, [FromQuery] string fileType)
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _engagementInterface.GenerateDocumentAsync(data, fileType);
                return File(fileBytes, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


