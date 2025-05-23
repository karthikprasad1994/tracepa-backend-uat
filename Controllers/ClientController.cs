


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private AuditInterface _AuditInterface;
        public ClientController(AuditInterface AuditInterface)
        {
            _AuditInterface = AuditInterface;

        }

        // GET: api/<ClientController>
        //[HttpGet("GetDropdownData")]
        //public async Task<IActionResult> GetDropdownData([FromQuery] int companyId)
        //{
        //    var result = await _AuditInterface.GetCustomerAuditDropdownAsync(companyId);
        //    return Ok(result);
        //}
        [HttpGet("GetDateFormat")]
        public async Task<IActionResult> GetDateFormat(string connectionKey, int companyId, string configKey)
        {
            try
            {
                var format = await _AuditInterface.GetDateFormatAsync(connectionKey, companyId, configKey);

                return Ok(new
                {
                    status = 200,
                    message = "Date format retrieved successfully.",
                    data = format
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while retrieving the date format.",
                    error = ex.Message
                });
            }
        }

       

            [HttpGet("GetLoeTemplateSignedOn")]
            public async Task<IActionResult> GetLoeTemplateSignedOn(
                string connectionStringName, int companyId, int auditTypeId, int customerId, int yearId, string dateFormat)
            {
                try
                {
                    var approvedOn = await _AuditInterface.GetLoeTemplateSignedOnAsync(
                        connectionStringName, companyId, auditTypeId, customerId, yearId, dateFormat);

                    return Ok(new
                    {
                        status = 200,
                        message = "Record retrieved successfully.",
                        data = approvedOn
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        status = 500,
                        message = "An error occurred while processing your request.",
                        error = ex.Message
                    });
                }
            }


        [HttpGet("GetCustomerFinancialYear")]
        public async Task<IActionResult> GetCustomerFinancialYear(
       string connectionKey, int companyId, int customerId)
        {
            try
            {
                var result = await _AuditInterface.GetCustomerFinancialYearAsync(connectionKey, companyId, customerId);

                return Ok(new
                {
                    status = 200,
                    message = "Financial year retrieved successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while processing your request.",
                    error = ex.Message
                });
            }
        }

        

            [HttpGet("GetReportTypes")]
            public async Task<IActionResult> GetReportTypes(string connectionKey, int companyId)
            {
                try
                {
                    var reportTypes = await _AuditInterface.GetReportTypesAsync(connectionKey, companyId);

                    return Ok(new
                    {
                        status = 200,
                        message = "Report types retrieved successfully.",
                        data = reportTypes
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        status = 500,
                        message = "An error occurred while processing your request.",
                        error = ex.Message
                    });
                }
            }
        



        [HttpGet("ActiveCustomers")]
        public async Task<IActionResult> GetActiveCustomers([FromQuery] int companyId)
        {
            try
            {
                var customers = await _AuditInterface.LoadActiveCustomersAsync(companyId);

                if (customers == null || !customers.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No active customers found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Active customers loaded successfully.",
                    Data = customers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching active customers.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("LoadScheduledAuditNos")]
        public async Task<IActionResult> GetScheduledAuditNos(
     [FromQuery] string connectionStringName,
     [FromQuery] int companyId,
     [FromQuery] int financialYearId,
     [FromQuery] int customerId)
        {
            try
            {
                var result = await _AuditInterface.LoadScheduledAuditNosAsync(
                    connectionStringName, companyId, financialYearId, customerId);

                if (!result.Any())
                    return NotFound(new { StatusCode = 404, Message = "No scheduled audits found." });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Scheduled audits fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Error occurred while fetching scheduled audits.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("LoadAllReportTypeDetailsDRL")]
        public async Task<IActionResult> GetReportTypeDetails(
        [FromQuery] string connectionStringName,
        [FromQuery] int companyId,
        [FromQuery] int templateId,
        [FromQuery] string auditNo)
        {
            try
            {
                var result = await _AuditInterface.LoadAllReportTypeDetailsDRLAsync(
                    connectionStringName, companyId, templateId, auditNo);

                if (!result.Any())
                    return NotFound(new { StatusCode = 404, Message = "No report types found." });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Report types fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching report types.",
                    Error = ex.Message
                });
            }
        }



        [HttpGet("GetDrl")]
        public async Task<IActionResult> LoadDrlOptions([FromQuery] int compId, [FromQuery] string type, [FromQuery] string auditNo)
        {
            try
            {
                var result = await _AuditInterface.LoadDRLClientSideAsync(compId, type, auditNo);

                return Ok(new
                {
                    statusCode = 200,
                    message = "DRL options loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Failed to load DRL options.",
                    error = ex.Message
                });
            }
        }




        [HttpGet("GetCustomerUserEmails")]
        public async Task<IActionResult> GetCustomerUserEmails(
    [FromQuery] string connectionStringName,
    [FromQuery] int companyId,
    [FromQuery] int customerId)
        {
            try
            {
                var result = await _AuditInterface.GetCustAllUserEmailsAsync(connectionStringName, companyId, customerId);

                if (!result.Any())
                    return NotFound(new { StatusCode = 404, Message = "No emails found." });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Customer user emails fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Error while fetching customer user emails.",
                    Error = ex.Message
                });
            }
        }


        [HttpGet("GetFinancialYearsUpTo")]
        public async Task<IActionResult> GetFinancialYearsUpTo(
    [FromQuery] string connectionStringName,
    [FromQuery] int companyId,
    [FromQuery] int incrementBy)
        {
            try
            {
                var result = await _AuditInterface.GetAddYearTo2DigitFinancialYearAsync(connectionStringName, companyId, incrementBy);

                if (!result.Any())
                    return NotFound(new { StatusCode = 404, Message = "No financial years found." });

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Financial years fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Error fetching financial years.",
                    Error = ex.Message
                });
            }
        }


        [HttpGet("GetSelfAttachId")]
        public async Task<IActionResult> GetDuringSelfAttachId(
    [FromQuery] string connectionStringName,
    [FromQuery] int companyId,
    [FromQuery] int yearId,
    [FromQuery] int customerId,
    [FromQuery] int auditId,
    [FromQuery] int drlId)
        {
            try
            {
                var attachId = await _AuditInterface.GetDuringSelfAttachIdAsync(connectionStringName, companyId, yearId, customerId, auditId, drlId);
                return Ok(new { StatusCode = 200, AttachId = attachId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Error fetching attachment ID.", Error = ex.Message });
            }
        }

        [HttpGet("GetAllDRLDescriptions")]
        public async Task<IActionResult> GetAllDRLDescriptions(string connectionStringName, int companyId)
        {
            try
            {
                var result = await _AuditInterface.LoadAllDRLDescriptionsAsync(connectionStringName, companyId);

                return Ok(new
                {
                    statusCode = 200,
                    message = "All DRL descriptions fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving DRL descriptions.",
                    data = (object)null
                });
            }
        }

        [HttpGet("during-self-attach-id")]
        public async Task<IActionResult> GetDuringSelfAttachId(
       [FromQuery] int companyId,
       [FromQuery] int yearId,
       [FromQuery] int customerId,
       [FromQuery] int auditId,
       [FromQuery] int drlId)
        {
            try
            {
                int attachId = await _AuditInterface.GetDuringSelfAttachIdAsync(companyId, yearId, customerId, auditId, drlId);

                return Ok(new
                {
                    statusCode = 200,
                    message = attachId > 0
                        ? "Attachment ID retrieved successfully."
                        : "No attachment found for the given criteria.",
                    data = attachId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the attachment ID.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("LoadDRLDescription")]
        public async Task<IActionResult> LoadDRLDescription(string connectionStringName, int companyId, int drlId)
        {
            try
            {
                var result = await _AuditInterface.LoadDRLDescriptionAsync(connectionStringName, companyId, drlId);

                return Ok(new
                {
                    statusCode = 200,
                    message = "DRL description fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Optionally log the error (ex)
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the DRL description.",
                    data = (object)null
                });
            }
        }


        [HttpPost("Insert/updateTemplate")]
        public async Task<IActionResult> SaveOrUpdate([FromQuery] string connectionKey, [FromBody] LoETemplateDetailInputDto dto)
        {
            try
            {
                var (id, action) = await _AuditInterface.SaveOrUpdateLOETemplateDetailsAsync(connectionKey, dto);

                string message = action == "Inserted"
                    ? "LOE Template details inserted successfully."
                    : "LOE Template details updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Id = id,
                    Action = action,
                    Message = message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving LOE Template details.",
                    Error = ex.Message
                });
            }
        }


        [HttpGet("GetDRLAttachmentInfo")]
        public async Task<IActionResult> GetDRLAttachmentInfo(int compId, int customerId, int drlId)
        {
            try
            {
                var attachments = await _AuditInterface.GetDRLAttachmentInfoAsync(compId, customerId, drlId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data fetched successfully",
                    Data = attachments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = $"Error retrieving attachment info: {ex.Message}",
                    Data = new List<object>() // Empty data on error
                });
            }
        }

        //[HttpPost("GenerateDRLReport")]
        //public async Task<IActionResult> GenerateDRLReport([FromForm] DRLReportRequest request)
        //{
        //    try
        //    {
        //        var result = await _drlReportService.SaveDRLLogWithAttachmentAsync(request);

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "DRL log saved and file generated successfully.",
        //            Data = new
        //            {
        //                result.DrlLogId,
        //                result.AttachmentId,
        //                result.FilePath
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error generating DRL report");
        //        return StatusCode(StatusCodes.Status500InternalServerError, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while generating the DRL report."
        //        });
        //    }
        //}

        [HttpGet("LoadAttachments")]
        public async Task<IActionResult> LoadAttachments(string connectionStringName, int companyId, int attachId, int Drlid)
        {
            try
            {
                var result = await _AuditInterface.LoadAttachmentsAsync(connectionStringName, companyId, attachId, Drlid);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = result.Count > 0 ? "Attachments loaded successfully." : "No attachments found.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while loading attachments.",
                    Error = ex.Message
                });
            }
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadAttachment([FromForm] AddFileDto dto)
        {


            try
            {
                var result = await _AuditInterface.UploadAndSaveAttachmentAsync(dto);

                if (result.StartsWith("Error"))
                {
                    return StatusCode(500, result); // Internal Server Error
                }

                return Ok("File uploaded, Customer details saved Successfully"); // Success
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpGet("LoadLOEHeading")]
        public async Task<IActionResult> LoadLOEHeading([FromQuery] string sFormName, [FromQuery] int compId, [FromQuery] int reportTypeId, [FromQuery] int loeTemplateId)
        {
            try
            {
                var result = await _AuditInterface.LoadLOEHeadingAsync(sFormName, compId, reportTypeId, loeTemplateId);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while loading LOE headings.");
            }
        }



        [HttpGet("GetWorkpapers")]
        public async Task<IActionResult> GetWorkpapers(
            [FromQuery] string connectionStringName,
            [FromQuery] int companyId,
            [FromQuery] int auditId)
        {
            try
            {
                var result = await _AuditInterface.GetAuditWorkpaperNosAsync(connectionStringName, companyId, auditId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (if you have a logger)
                return StatusCode(500, "An error occurred while retrieving the workpapers.");
            }
        }

        [HttpGet("GetChecklists")]
        public async Task<IActionResult> GetChecklists([FromQuery] string connectionStringName, [FromQuery] int companyId)
        {
            try
            {
                var checklists = await _AuditInterface.LoadWorkpaperChecklistsAsync(connectionStringName, companyId);
                return Ok(checklists);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error fetching checklist data.");
            }
        }


        [HttpGet("GetDrlAttachments")]
        public async Task<IActionResult> GetDrlAttachments(
            [FromQuery] string connectionStringName,
            [FromQuery] int companyId,
            [FromQuery] string categoryType,
            [FromQuery] string auditNo,
            [FromQuery] int auditId)
        {
            try
            {
                var result = await _AuditInterface.LoadOnlyDRLWithAttachmentsAsync(connectionStringName, companyId, categoryType, auditNo, auditId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while loading DRL attachment data.");
            }
        }

        [HttpPost("SaveWorkpaper")]
        public async Task<IActionResult> SaveWorkpaper([FromBody] Dto.Audit.WorkpaperDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid workpaper data.");

            try
            {
                // Check if WorkpaperRef already exists for a different WorkpaperId (to allow update)
                bool exists = await _AuditInterface.CheckWorkpaperRefExists(dto.AuditId, dto.WorkpaperRef, dto.WorkpaperId);
                if (exists && dto.WorkpaperId == 0) // 0 means it's an insert, so prevent insert if ref exists
                    return Conflict("Workpaper reference already exists.");

                // Generate workpaper number if it's a new entry
                string workpaperNo = dto.WorkpaperId == 0 ? await _AuditInterface.GenerateWorkpaperNo(dto.AuditId) : string.Empty;

                // Save the workpaper (insert or update)
                int result = await _AuditInterface.SaveWorkpaperAsync(dto, workpaperNo);

                // Check if it's an update or insert based on the returned result
                string message = result == dto.WorkpaperId ? "Workpaper updated successfully." : "Workpaper inserted successfully.";

                return Ok(new
                {
                    Message = message,
                    WorkpaperId = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("GetWorkpapersDetails")]
        public async Task<IActionResult> GetWorkpapers([FromQuery] int auditId, [FromQuery] int companyId)
        {
            try
            {
                var data = await _AuditInterface.LoadConductAuditWorkPapersAsync(companyId, auditId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetStandardAuditHeadings")]
        public async Task<IActionResult> GetStandardAuditHeadings([FromQuery] int auditId, [FromQuery] int companyId)
        {
            try
            {
                var data = await _AuditInterface.LoadAllStandardAuditHeadingsAsync(companyId, auditId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetWorkpaperNumbers")]
        public async Task<IActionResult> GetWorkpaperNumbers([FromQuery] int auditId, [FromQuery] int companyId)
        {
            try
            {
                var data = await _AuditInterface.GetConductAuditWorkpaperNosAsync(companyId, auditId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("AssignWorkpaperToCheckpoint")]
        public async Task<IActionResult> AssignWorkpaperToCheckpoint([FromBody] AssignWorkpaperDto dto)
        {
            try
            {
                await _AuditInterface.AssignWorkpaperToCheckPointAsync(dto);
                return Ok(new { message = "Workpaper assigned successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetAuditCheckpoints")]
        public async Task<IActionResult> LoadAuditCheckpoints([FromQuery] int companyId, [FromQuery] int auditId, [FromQuery] int empId, [FromQuery] bool isPartner, [FromQuery] int headingId, [FromQuery] string heading)
        {
            try
            {
                var result = await _AuditInterface.LoadSelectedAuditCheckPointDetailsAsync(companyId, auditId, empId, isPartner, headingId, heading);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("updateScheduleCheckPointRemarksAnnexure")]
        public async Task<IActionResult> UpdateScheduleCheckPointRemarksAnnexureAsync([FromBody] UpdateScheduleCheckPointDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                // Call the service method to update the record
                await _AuditInterface.UpdateScheduleCheckPointRemarksAnnexureAsync(dto);

                // Return a successful response
                return Ok(new { message = "Schedule Checkpoint updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the error (you may use a logger here)
                return StatusCode(500, new { message = "An error occurred while updating the schedule checkpoint.", details = ex.Message });
            }
        }


        [HttpPost("upload-attachment-without-email")]
        public async Task<IActionResult> UploadAttachmentWithoutEmail([FromForm] AddFileDto dto)
        {
            var result = await _AuditInterface.UploadAndSaveAttachmentsAsync(dto);

            if (result.StartsWith("Error"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetCustomerInvoicedetails")]
        public async Task<IActionResult> GetInvoiceDetails([FromQuery] int companyId, [FromQuery] int customerId)
        {
            var result = await _AuditInterface.GetCustomerDetailsForInvoiceAsync(companyId, customerId);

            if (result == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "Customer not found"
                });
            }

            return Ok(new
            {
                statusCode = 200,
                message = "Customer invoice details fetched successfully",
                data = result
            });
        }


        [HttpGet("GetCustomerData")]
        public async Task<IActionResult> GetCustomerData(
        [FromQuery] int companyId,
        [FromQuery] int customerId,
        [FromQuery] int reportTypeId)
        {
            try
            {
                var result = await _AuditInterface.GetCustomerDetailsWithTemplatesAsync(companyId, customerId, reportTypeId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Customer not found or  data available.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer data fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Failed to fetch customer  data.",
                    error = ex.Message
                });
            }
        }

       

            [HttpGet("GenerateFiles")]
            public async Task<IActionResult> GenerateCustomerReport(int companyId, int customerId, int reportTypeId)
            {
                try
                {
                    var (wordPath, pdfPath) = await _AuditInterface.GenerateCustomerReportFilesAsync(companyId, customerId, reportTypeId);

                    var response = new ReportFileResponseDto
                    {
                        StatusCode = 200,
                        Message = "Report files generated successfully.",
                        WordFilePath = wordPath,
                        PdfFilePath = pdfPath
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ReportFileResponseDto
                    {
                        StatusCode = 500,
                        Message = "Failed to generate report: " + ex.Message,
                        WordFilePath = null,
                        PdfFilePath = null
                    });
                }
            }
        




        [HttpPost("GenerateFile")]
        public async Task<IActionResult> GenerateDrlReport([FromBody] DRLRequestDto request, [FromQuery] string format = "pdf")
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _AuditInterface.GenerateAndLogDRLReportAsync(request, format);

                return File(fileBytes, contentType, fileName);
            }
            catch
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Failed to generate report"
                });
            }
        }
    }


}


