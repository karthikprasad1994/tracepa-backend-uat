


using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Table.PivotTable;
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
            string connectionStringName, int companyId, int auditTypeId, int customerId, int yearId)
        {
            try
            {
                var approvedOn = await _AuditInterface.GetLoeTemplateSignedOnAsync(
                    connectionStringName, companyId, auditTypeId, customerId, yearId);

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


        [HttpGet("GetScheduleDetails")]
        public async Task<IActionResult> GetMergedScheduleDetails([FromQuery] int customerId, [FromQuery] int auditId)
        {
            try
            {
                var result = await _AuditInterface.GetScheduleMergedDetailsAsync(customerId, auditId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No data found for the given customer ID and audit ID"
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule details fetched successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"An error occurred while fetching schedule details: {ex.Message}"
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
        [HttpPost("insert-update-template")]
        public async Task<IActionResult> SaveOrUpdateMultiple(
    [FromQuery] string connectionKey,
    [FromBody] LoETemplateDetailBatchDto batchDto)
        {
            try
            {
                var result = await _AuditInterface.SaveOrUpdateLOETemplateDetailsAsync(connectionKey, batchDto.Items);

                return Ok(new
                {
                    statusCode = 200,
                   // message = "LOE Template details processed successfully.",
                    records = result.Select(r => new
                    {
                        id = r.Id,
                        action = r.Action,
                        message = r.Action == "Inserted"
                            ?  "Template details inserted successfully."
                            :  "Template details updated successfully."

                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while saving LOE Template details.",
                    error = ex.Message
                });
            }
        }


        //[HttpPost("insert-update-template")]
        //public async Task<IActionResult> SaveOrUpdate([FromQuery] string connectionKey, [FromBody] LoETemplateDetailInputDto dto)
        //{
        //    try
        //    {
        //        var (id, action) = await _AuditInterface.SaveOrUpdateLOETemplateDetailsAsync(connectionKey, dto);

        //        string message = action == "Inserted"
        //            ? "LOE Template details inserted successfully."
        //            : "LOE Template details updated successfully.";

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            id,
        //            action,
        //            message
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while saving LOE Template details.",
        //            error = ex.Message
        //        });
        //    }
        //}


        [HttpGet("GetDRLDescription")]
            public async Task<IActionResult> GetDRLDescription(int companyId, int drlId)
            {
                try
                {
                var description = await _AuditInterface.GetDRLDescriptionByIdAsync(companyId, drlId);

                    // Call your service method
                    

                    return Ok(new
                    {
                        statusCode = 200,
                        message = "DRL description fetched successfully.",
                        comments = description
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        statusCode = 500,
                        message = "An error occurred while fetching the DRL description.",
                        error = ex.Message
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
        public async Task<IActionResult> LoadAttachments(string connectionStringName, int companyId, int attachId,int ReportType)
        {
            try
            {
                var result = await _AuditInterface.LoadAttachmentsAsync(connectionStringName, companyId, attachId, ReportType);

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
            var result = await _AuditInterface.UploadAndSaveAttachmentAsync(dto);

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, new { statusCode = 500, message = result });
            }

            return Ok(new { statusCode = 200, message = result });
        }


        [HttpGet("GetReportHistory")]
        public async Task<IActionResult> GetReportHistory([FromQuery] ReportHistoryCommentsDto dto)
        {
            try
            {
                var result = await _AuditInterface.GetReportHistoryComments(dto);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Report History fetched Successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = ex.Message
                });
            }
        }




        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadAttachment([FromForm] AddFileDto dto)
        //{
        //    try
        //    {
        //        var result = await _AuditInterface.UploadAndSaveAttachmentAsync(dto);

        //        if (result.StartsWith("Error"))
        //        {
        //            return StatusCode(500, new
        //            {
        //                statusCode = 500,
        //                message = result
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "File uploaded, Customer details saved Successfully"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = $"Error: {ex.Message}"
        //        });
        //    }
        //}


        [HttpGet("GetDrlInfo")]
        public async Task<IActionResult> LoadPostAndPreAuditRemarks(
      [FromQuery] string connectionStringName,
      [FromQuery] int customerId,
      [FromQuery] int auditId,
      [FromQuery] int reportType)
        {
            try 
            {
                //var connectionString = _configuration.GetConnectionString(connectionStringName);

                var results = await _AuditInterface.LoadPostAndPreAuditAsync(connectionStringName, customerId, auditId, reportType);

                return Ok(new
                {
                    statusCode = 200,
                    message = results.Any() ? "Remarks history loaded successfully." : "No remarks found.",
                    data = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading remarks history.",
                    error = ex.Message
                });
            }
        }




        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateDrlStatus([FromBody] UpdateDrlStatusDto dto)
        {
            var (isSuccess, message) = await _AuditInterface.UpdateDrlStatusAsync(dto);

            if (isSuccess)
                return Ok(new { StatusCode = 200, Message = message });
            else
                return StatusCode(500, new { StatusCode = 500, Message = message });
        }


        //[HttpPost("DuringAuditupload")]
        //public async Task<IActionResult> BeginAuditUploadWithReportTypeAsync([FromForm] AddFileDto dto)
        //{
        //    try
        //    {
        //        var result = await _AuditInterface.BeginAuditUploadWithReportTypeAsync(dto);

        //        if (result.StartsWith("Error"))
        //        {
        //            return StatusCode(500, new
        //            {
        //                statusCode = 500,
        //                message = result
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "File uploaded, Customer details saved Successfully"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = $"Error: {ex.Message}"
        //        });
        //    }
        //}


        [HttpGet("GetDRLDetails")]
        public async Task<IActionResult> LoadDRLDetails([FromQuery] int compId, [FromQuery] int auditNo)
        {
            try
            {
                var data = await _AuditInterface.LoadDRLdgAsync(compId, auditNo);

                return Ok(new
                {
                    Success = true,
                    Message = "DRL details loaded successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while loading DRL details.",
                    Error = ex.Message
                });
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


        //[HttpGet("GetDrlAttachments")]
        //public async Task<IActionResult> GetDrlAttachments(
        //    [FromQuery] string connectionStringName,
        //    [FromQuery] int companyId,
        //    [FromQuery] string categoryType,
        //    [FromQuery] string auditNo,
        //    [FromQuery] int auditId)
        //{
        //    try
        //    {
        //        var result = await _AuditInterface.LoadOnlyDRLWithAttachmentsAsync(connectionStringName, companyId, categoryType, auditNo, auditId);
        //        return Ok(result);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "An error occurred while loading DRL attachment data.");
        //    }
        //}

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
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Success = false,
                    Message = result
                });
            }

            return Ok(new
            {
                Status = 200,
                Success = true,
                Message = result
            });
        }

        //[HttpPost("SaveAll")]
        //public async Task<IActionResult> SaveAll([FromBody]
        //
        //Request request)
        //{
        //    try
        //    {
        //        int drlLogId = await _AuditInterface.SaveAuditAllAsync(
        //            request.Dto,
        //            request.RequestedId,
        //            request.Module,
        //            request.Form,
        //            request.Event,
        //            request.MasterId,
        //            request.MasterName,
        //            request.SubMasterId,
        //            request.SubMasterName,
        //            request.AttachId);

        //        return Ok(new { Success = true, DrlLogId = drlLogId });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Success = false, Message = ex.Message });
        //    }
        //}



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


        [HttpGet("CustomerUsers")]
        public async Task<IActionResult> GetCustomerUsers(int customerId)
        {
            try
            {
                var result = await _AuditInterface.GetAllCustomerUsersAsync(customerId);

                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        status = 200,
                        message = "Customer users fetched successfully.",
                        data = result
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No users found for the specified customer.",
                        data = Enumerable.Empty<CustomerUserDto>()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching customer users.",
                    error = ex.Message
                });
            }
        }



        [HttpPost("GenerateFile")]
        public async Task<IActionResult> GenerateDrlReport([FromBody] DRLRequestDto request, [FromQuery] string format = "pdf")
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _AuditInterface.GenerateDRLReportWithoutSavingAsync(request, "pdf");

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




        [HttpPost("SaveAuditData")]
        public async Task<IActionResult> SaveAuditDrl([FromBody] InsertAuditRemarksDto dto)
        {
            try
            {
                var (drlId, isInsert) = await _AuditInterface.SaveAuditDataAsync(dto);
                string message = isInsert ? "Record inserted successfully." : "Record updated successfully.";
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving audit data.", error = ex.Message });
            }
        }



        [HttpGet("GetDrlId")]
        public async Task<IActionResult> GetRequestedIdByExportType([FromQuery] int exportType)
        {
          

            var requestedId = await _AuditInterface.GetRequestedIdByExportTypeAsync(exportType);
            return Ok(new { RequestedId = requestedId });
        }


        [HttpGet("GetAttachId")]
        public async Task<IActionResult> GetMaxAttachmentId(
    [FromQuery] int customerId,    // Customer ID (SAR_SAC_ID)
    [FromQuery] int auditId,       // Audit ID (SAR_SA_ID)
    [FromQuery] int yearId,        // Year ID (sar_Yearid)
    [FromQuery] int exportType)    // Export Type (1 or 3)
        {


            var maxId = await _AuditInterface.GetMaxAttachmentIdAsync(customerId, auditId, yearId, exportType);

            return Ok(new { MaxAttachmentId = maxId });
        }

        [HttpGet("GetAllAttachIds")]
        public async Task<IActionResult> GetDistinctAttachIds([FromQuery] int companyId)
        {
            if (companyId <= 0)
            {
                return BadRequest("Invalid companyId.");
            }

            string connectionStringName = "DefaultConnection";

            var attachIds = await _AuditInterface.GetAttachIdsAsync(connectionStringName, companyId);
            if (attachIds == null || !attachIds.Any())
            {
                return NotFound(new { Message = "No attachment IDs found for the specified company." });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Attachment IDs fetched successfully.",
                Data = attachIds
            });
        }

        [HttpGet("get-max-attachment-id")]
        public async Task<IActionResult> GetMaxAttachmentId([FromQuery] GetMaxAttachmentIdRequest request)
        {
            var maxId = await _AuditInterface.GetMaxAttachmentIdAsync(request);
            return Ok(new
            {
                statusCode = 200,
                message = "Max Attachment ID fetched successfully.",
                data = maxId
            });
        }





        [HttpGet("GetStandardAttachments")]
        public async Task<IActionResult> LoadAllAttachments(
                [FromQuery] string connectionStringName,
                [FromQuery] int companyId,
                [FromQuery] int attachId,int ReportType)

        {
            try
            {
                var data = await _AuditInterface.LoadAttachmentsAsync(connectionStringName, companyId, attachId, ReportType);

                if (data == null || !data.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No attachments found.",
                        Data = new List<AttachmentDto>()
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Attachments loaded successfully.",
                    Data = data
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




        [HttpGet("load-checkpoints")]
        public async Task<IActionResult> LoadSelectedStandardAuditCheckPointDetails(
            [FromQuery] string connectionKey,
            [FromQuery] int compId,
            [FromQuery] int auditId,
            [FromQuery] int empId,
            [FromQuery] bool isPartner,

            [FromQuery] int headingId = 0,
            [FromQuery] string heading = "")
        {
            try
            {
                var result = await _AuditInterface.LoadSelectedStandardAuditCheckPointDetailsAsync(
                    connectionKey, compId, auditId, empId, isPartner, headingId, heading);

                return Ok(new
                {
                    status = 200,
                    message = "Checkpoints loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while loading checkpoints.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("GetWorkpaperDetails")]
        public async Task<IActionResult> GetWorkpaperDetails(
      [FromQuery] string connStrName,
      [FromQuery] int compId,
      [FromQuery] int auditId,
      [FromQuery] int workpaperId)
        {
            try
            {
                var result = await _AuditInterface.LoadSelectedConductAuditWorkPapersDetailsAsync(connStrName, compId, auditId, workpaperId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "Workpaper details not found."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Workpaper details loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while retrieving workpaper details.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("remarks-history")]
        public async Task<IActionResult> GetRemarksHistory(
       [FromQuery] string connStrName,
       [FromQuery] int compId,
       [FromQuery] int auditId,
       [FromQuery] int reportType,
       [FromQuery] int customerId)
        {
            try
            {
                var result = await _AuditInterface.LoadSelectedDRLCheckPointRemarksHistoryDetailsAsync(
                    connStrName, compId, auditId, reportType, customerId);

                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Remarks history fetched successfully.",
                        Data = result
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        Status = "NotFound",
                        Message = "No remarks history found.",
                        Data = new List<DrlRemarksHistoryDto>()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"An error occurred: {ex.Message}"
                });
            }

        }


       

            [HttpPost("get-sacid")]
            public async Task<IActionResult> GetSACId([FromBody] CheckPointIdentifierDto dto)
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.CheckPointId))
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Invalid input data.",
                        data = (int?)null
                    });
                }

                var sacId = await _AuditInterface.GetSACIdAsync(dto);

                if (sacId.HasValue)
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "SAC_ID retrieved successfully.",
                        data = sacId.Value
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "SAC_ID not found for the given input.",
                        data = (int?)null
                    });
                }

            }

        [HttpPut("UpdateAttachmentDescription")]
        public async Task<IActionResult> UpdateAllRemarks([FromBody] UpdateAttachmentDescriptionDto dto)
        {
            try
            {
                var result = await _AuditInterface.UpdateAttachmentDescriptionOnlyAsync(dto);

                if (result)
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Description updated successfully"
                    });
                }

                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Failed to update description"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = $"Error: {ex.Message}"
                });
            }
        }

        [HttpPost("Local-upload-multiple")]
        public async Task<IActionResult> UploadMultiple([FromForm] LocalAttachmentDto dto)
        {
            if (dto.Files == null || !dto.Files.Any())
                return BadRequest("No files uploaded.");

            var ids = await _AuditInterface.SaveAttachmentsAsync(dto);
            return Ok(new { UploadedAttachmentIds = ids });
        }




    }

}

        




    







        



    


