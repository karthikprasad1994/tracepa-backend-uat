using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFilling;
using TracePca.Interface.Audit;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Interface.Master;

namespace TracePca.Controllers.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditSummaryController : ControllerBase
    {

        private AuditSummaryInterface _AuditSummaryInterface;
        private readonly Trdmyus1Context _dbcontext;
        private readonly EngagementPlanInterface _engagementInterface;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGoogleDriveService _googleDriveService;
        public AuditSummaryController(Trdmyus1Context dbcontext,AuditSummaryInterface AuditSummaryInterface, IHttpContextAccessor httpContextAccessor, EngagementPlanInterface engagementInterface, IGoogleDriveService googleDriveService)
        {
            _AuditSummaryInterface = AuditSummaryInterface;
            _dbcontext = dbcontext;
            _engagementInterface = engagementInterface;
            _httpContextAccessor = httpContextAccessor;
            _googleDriveService = googleDriveService;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet("LoadCustomer")]
        public async Task<IActionResult> LoadCustomer(int compId)
        {
            var dropdownData = await _AuditSummaryInterface.LoadCustomerDataAsync(compId);

            return Ok(new
            {
                statusCode = 200,
                message = "Dropdowns fetched successfully",
                data = dropdownData
            });
        }


        [HttpGet("LoadAudiNoDetails")]
        public async Task<IActionResult> LoadAudiNoDetails(int iCustID, int compId, int Yearid, int userid)
        {
            var dropdownData = await _AuditSummaryInterface.LoadAuditNoDataAsync(iCustID, compId, Yearid, userid);

            return Ok(new
            {
                statusCode = 200,
                message = "Dropdowns fetched successfully",
                data = dropdownData
            });
        }



        [HttpGet("GetAuditDetails")]
        public async Task<IActionResult> GetAuditDetails([FromQuery] int compId, [FromQuery] int customerId, [FromQuery] int auditNo)
        {
            try
            {
                var auditDetails = await _AuditSummaryInterface.GetAuditDetailsAsync(compId, customerId, auditNo);

                if (auditDetails != null && auditDetails.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Audit Details fetched successfully.",
                        data = new
                        {
                            auditDetails = auditDetails
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Audit Details found for the selected filters.",
                        data = new
                        {
                            auditDetails = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        auditDetails = new List<object>()
                    }
                });
            }
        }



        [HttpGet("GetDocumentRequestSummary")]
        public async Task<IActionResult> GetDocumentRequestSummary([FromQuery] int compId, [FromQuery] int customerId, [FromQuery] int auditNo, [FromQuery] int yearId)
        {
            try
            {
                var documentSummary = await _AuditSummaryInterface.GetDocumentRequestSummaryAsync(compId, customerId, auditNo, yearId);

                if (documentSummary != null && documentSummary.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Document Request Summary details fetched successfully.",
                        data = new
                        {
                            documentSummary = documentSummary
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Document Request Summary details found for the selected filters.",
                        data = new
                        {
                            documentSummary = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        documentSummary = new List<object>()
                    }
                });
            }
        }



        [HttpGet("GetDocumentRequestSummaryDuringAudit")]
        public async Task<IActionResult> GetDocumentRequestSummaryDuringAudit([FromQuery] int compId, [FromQuery] int customerId, [FromQuery] int auditNo, [FromQuery] int requestId, [FromQuery] int yearId)
        {
            try
            {
                var documentSummaryDuringAudit = await _AuditSummaryInterface.GetDocumentRequestSummaryDuringAuditAsync(compId, customerId, auditNo, requestId, yearId);

                if (documentSummaryDuringAudit != null && documentSummaryDuringAudit.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Document Request Summary During Audit details fetched successfully.",
                        data = new
                        {
                            documentSummaryDuringAudit = documentSummaryDuringAudit
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Document Request Summary During Audit details found for the selected filters.",
                        data = new
                        {
                            documentSummaryDuringAudit = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        documentSummaryDuringAudit = new List<object>()
                    }
                });
            }
        }


        [HttpGet("GetDocumentRequestSummaryCompletionAudit")]
        public async Task<IActionResult> GetDocumentRequestSummaryCompletionAudit([FromQuery] int compId, [FromQuery] int customerId, [FromQuery] int auditNo, [FromQuery] int requestId, [FromQuery] int yearId)
        {
            try
            {
                var documentSummaryDuringAudit = await _AuditSummaryInterface.GetDocumentRequestSummaryCompletionAuditAsync(compId, customerId, auditNo, requestId, yearId);

                if (documentSummaryDuringAudit != null && documentSummaryDuringAudit.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Document Request Summary Completion Audit details fetched successfully.",
                        data = new
                        {
                            documentSummaryDuringAudit = documentSummaryDuringAudit
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Document Request Summary Completion Audit details found for the selected filters.",
                        data = new
                        {
                            documentSummaryDuringAudit = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        documentSummaryDuringAudit = new List<object>()
                    }
                });
            }
        }


        [HttpGet("GetAuditProgramSummary")]
        public async Task<IActionResult> GetAuditProgramSummary([FromQuery] int compId, [FromQuery] int auditNo)
        {
            try
            {
                var auditProgramSummary = await _AuditSummaryInterface.GetAuditProgramSummaryAsync(compId, auditNo);

                if (auditProgramSummary != null && auditProgramSummary.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Audit Program Summary details fetched successfully.",
                        data = new
                        {
                            auditProgramSummary = auditProgramSummary
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Audit Program Summary details found for the selected filters.",
                        data = new
                        {
                            auditProgramSummary = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        auditProgramSummary = new List<object>()
                    }
                });
            }
        }


        [HttpGet("GetWorkspaceSummary")]
        public async Task<IActionResult> GetWorkspaceSummary([FromQuery] int compId, [FromQuery] int auditNo)
        {
            try
            {
                var workspaceSummary = await _AuditSummaryInterface.GetWorkspaceSummaryAsync(compId, auditNo);

                if (workspaceSummary != null && workspaceSummary.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Workspace Summary details fetched successfully.",
                        data = new
                        {
                            workspaceSummary = workspaceSummary
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Workspace Summary details found for the selected filters.",
                        data = new
                        {
                            workspaceSummary = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        workspaceSummary = new List<object>()
                    }
                });
            }
        }



        [HttpGet("GetCAMDetails")]
        public async Task<IActionResult> GetCAMDetails([FromQuery] int compId, [FromQuery] int auditNo)
        {
            try
            {
                var camDetails = await _AuditSummaryInterface.GetCAMDetailsAsync(compId, auditNo);

                if (camDetails != null && camDetails.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Critical Audit Matter details fetched successfully.",
                        data = new
                        {
                            camDetails = camDetails
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Critical Audit Matter details found for the selected filters.",
                        data = new
                        {
                            camDetails = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        camDetails = new List<object>()
                    }
                });
            }
        }


        [HttpPost("UpdateStandardAuditASCAMdetails/{sacm_pkid}/{sacm_sa_id}")]
        public async Task<IActionResult> UpdateStandardAuditASCAMdetails(int sacm_pkid, int sacm_sa_id, [FromBody] UpdateStandardAuditASCAMdetailsDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Invalid asset details.");
            }

            try
            {
                // Call service method to update asset details
                await _AuditSummaryInterface.UpdateStandardAuditASCAMdetailsAsync(sacm_pkid, sacm_sa_id, updateDto);

                // Return success response
                return Ok(new { message = "Standard Audit ASCAM details updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                // _logger.LogError(ex, "Error updating asset details.");

                // Return error responseg
                return StatusCode(500, new { message = "An error occurred while updating asset details.", error = ex.Message });
            }
        }



        [HttpPost("UploadCMAAttachments")]
        public async Task<IActionResult> UploadCMAAttachments([FromForm] CMADtoAttachment dto)
        {

            try
            {
                var result = await _AuditSummaryInterface.UploadCMAAttachmentsAsync(dto);
                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "File Upload Successfully.",
                        data = new
                        {
                            AttachPKID = Convert.ToInt32(result)
                        }
                    });
				}
				else
				{
					return NotFound(new
					{
						statusCode = 500,
						message = "Failed to upload.",
						data = new
						{
							result = new List<object>()
						}
					});
				}


				//   if (result.Contains("Error"))
				//            {
				//                return StatusCode(500, result); // Internal Server Error
				//            }
				//            dto.AttachPKID = Convert.ToInt32(result.Replace("Success|", "").ToString());


				//return Ok(dto.AttachPKID); // Success
			}
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetCAMAttachmentDetails")]
        public async Task<IActionResult> GetCAMAttachmentDetails(int AttachID)
        {
            try
            {
                var camDetails = await _AuditSummaryInterface.GetCAMAttachmentDetailsAsync(AttachID);

                if (camDetails != null && camDetails.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Critical Audit Matter Attachment details fetched successfully.",
                        data = new
                        {
                            camDetails = camDetails
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Critical Audit Matter Attachment details found for the selected filters.",
                        data = new
                        {
                            camDetails = new List<object>()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Internal server error: {ex.Message}",
                    data = new
                    {
                        camDetails = new List<object>()
                    }
                });
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
                if (docId <= 0)
                    return Ok(new { statusCode = 200, success = false, Message = "Document does not exist!" });

                var userEmail = _engagementInterface?.GetUserEmail();
                if (string.IsNullOrWhiteSpace(userEmail))
                    return Ok(new { statusCode = 200, success = false, Message = "Unable to determine user email." });

                var file = await _googleDriveService.GetFileByIdAsync(docId, userEmail);

                if (file == null)
                    return Ok(new { statusCode = 200, success = false, Message = "File not found or link unavailable." });

                var link =
                    (file.GetType().GetProperty("WebViewLink")?.GetValue(file, null)?.ToString()) ??
                    (file.GetType().GetProperty("WebContentLink")?.GetValue(file, null)?.ToString());

                if (string.IsNullOrEmpty(link))
                    return Ok(new { statusCode = 200, success = false, Message = "No viewable link found for this file." });

                return Ok(new { Status = "Success", Message = "File retrieved successfully.", Data = link });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while downloading the file: {ex.Message}" });
            }
        }

        [HttpPost("GenerateCAMReportAndGetURLPath")]
        public async Task<IActionResult> GenerateCAMReportAndGetURLPath(int compId, int auditId, string format = "pdf")
        {
            try
            {
                var url = await _AuditSummaryInterface.GenerateCAMReportAndGetURLPathAsync(compId, auditId, format);
                return Ok(new { statusCode = 200, message = "Audit Issues and Closure report generated successfully. Download URL is available.", fileUrl = url });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to generate report." });
            }
        }


		//[HttpPost("GetCAMAttachmentDetails")]
		//public async Task<IActionResult> GetCAMAttachmentDetails(int AttachID, [FromBody] CAMAttachmentDetailsDto dto)
		//{
		//	try
		//	{
		//		var camDetails = await _AuditSummaryInterface.GetCAMAttachmentDetailsAsync(AttachID, dto);

		//		if (camDetails != null && camDetails.Any())
		//		{
		//			return Ok(new
		//			{
		//				statusCode = 200,
		//				message = "Critical Audit Matter Attachment details fetched successfully.",
		//				data = new
		//				{
		//					camDetails = camDetails
		//				}
		//			});
		//		}
		//		else
		//		{
		//			return NotFound(new
		//			{
		//				statusCode = 404,
		//				message = "No Critical Audit Matter Attachment details found for the selected filters.",
		//				data = new
		//				{
		//					camDetails = new List<object>()
		//				}
		//			});
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500, new
		//		{
		//			statusCode = 500,
		//			message = $"Internal server error: {ex.Message}",
		//			data = new
		//			{
		//				camDetails = new List<object>()
		//			}
		//		});
		//	}
		//}

		 
		//private readonly HttpClient _httpClient;

		//[HttpGet]
		//public async Task<IActionResult> CallExternalApi()
		//{
		//	try
		//	{
		//		string apiUrl = "https://localhost:7090/api/Cabinet/LoadCabinet?deptId=5&userId=1&compID=1";

		//		//var response = await _AuditSummaryInterface.TrackPerformanceAsync(
		//		//	() => _httpClient.GetAsync(apiUrl),
		//		//	apiUrl,
		//		//	"GET"
		//		//);

		//		var response = await _AuditSummaryInterface.TrackPerformanceAsync(async () => await _httpClient.GetAsync(apiUrl),   
		//                        apiUrl,"GET");


		//		if (response is HttpResponseMessage httpResponse)
		//		{
		//			if (!httpResponse.IsSuccessStatusCode)
		//				return StatusCode((int)httpResponse.StatusCode);

		//			var content = await httpResponse.Content.ReadAsStringAsync();
		//			return Ok(content);
		//		}

		//		// Handle other return types if needed
		//		return BadRequest("Unexpected response type");
		//	}
		//	catch (HttpRequestException ex)
		//	{
		//		return StatusCode(500, $"External API call failed: {ex.Message}");
		//	}
		//}
	}
}
