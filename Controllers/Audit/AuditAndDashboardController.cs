using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.WebForms;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using TracePca.Dto.Audit;
using TracePca.Interface;
using TracePca.Interface.Audit;
using static Azure.Core.HttpHeader;
using Colors = QuestPDF.Helpers.Colors;

// For more information on enabling Web API for empty ? projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditAndDashboardController : ControllerBase
    {
        private AuditAndDashboardInterface _DashboardAndScheduleInterface;
        private AuditAndDashboardInterface _userService;
        private AuditAndDashboardInterface _quarterService;
        private AuditAndDashboardInterface _auditService;
        private AuditAndDashboardInterface _service;
        private AuditAndDashboardInterface _getAssignedCheckPoint;
        private AuditAndDashboardInterface _auditScheduleService;
        private AuditAndDashboardInterface _checklistService;
        private AuditAndDashboardInterface _auditChecklistService;
        private AuditAndDashboardInterface _CustomerSaveService;
        // GET: api/<AuditAndDashboardController>
        public AuditAndDashboardController(AuditAndDashboardInterface dashboardAndScheduleInterface)
        {
            _DashboardAndScheduleInterface = dashboardAndScheduleInterface;
            _userService = dashboardAndScheduleInterface;
            _quarterService = dashboardAndScheduleInterface;
            _auditService = dashboardAndScheduleInterface;
            _service = dashboardAndScheduleInterface;
            _getAssignedCheckPoint = dashboardAndScheduleInterface;
            _auditScheduleService = dashboardAndScheduleInterface;
            _checklistService = dashboardAndScheduleInterface;
            _auditChecklistService = dashboardAndScheduleInterface;
            _CustomerSaveService = dashboardAndScheduleInterface;
        }
        [HttpGet("GetDashboardAndAudit")]
        public async Task<IActionResult> GetDashboardAudit(
            int? id, int? customerId, int? compId, int? financialYearId, int? loginUserId)
        {
            var result = await _DashboardAndScheduleInterface.GetDashboardAuditAsync(id, customerId, compId, financialYearId, loginUserId);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result ?? new List<DashboardAndScheduleDto>()
            };

            return Ok(response);
        }


        [HttpGet("Partner or Reviewer or Assigned")]
        public async Task<IActionResult> GetUsersByRole(string role, int compId)    /* Role : Partner or Reviewer or Assigned */
        {
            var result = await _userService.GetUsersByRoleAsync(compId, role);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result ?? new List<UserDto>()
            };

            return Ok(response);
        }

        [HttpPost("audit-types")]
        public async Task<IActionResult> LoadAuditTypeCompliance([FromBody] AuditTypeRequestDto req)
        {
            var result = await _auditService.LoadAuditTypeComplianceDetailsAsync(req);
            return Ok(result);
        }

        [HttpGet("generate")]
        public IActionResult GenerateQuarters([FromQuery] DateTime? fromDate)
        {
            var result = _quarterService.GenerateQuarters(fromDate);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result
            };

            return Ok(response);
        }

        [HttpGet("headings")]
        public async Task<IActionResult> GetAuditTypeHeadings([FromQuery] int compId, [FromQuery] int auditTypeId)
        {
            var result = await _auditService.LoadAllAuditTypeHeadingsAsync(compId, auditTypeId);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result
            };

            return Ok(response);
        }
        [HttpGet("GetAssignedMembers")]
        public async Task<IActionResult> GetUsers([FromQuery] int companyId, [FromQuery] string userIds)
        {
            var users = await _userService.GetUsersAsync(companyId, userIds);
            return Ok(users);
        }
        [HttpGet("GetAssignedCheckpoints")]
        public async Task<IActionResult> GetAssignedCheckpoints(
    int compId, int auditId, int AuditTypeId, int custId, string sType, string heading, string? sCheckPoints)
        {
            try
            {
                var result = await _getAssignedCheckPoint.GetAssignedCheckpointsAndTeamMembersAsync(
                    compId, auditId, AuditTypeId, custId, sType, heading, sCheckPoints);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error occurred", error = ex.Message });
            }
        }

        [HttpPost("DeleteCheckpoints")]
        public async Task<IActionResult> DeleteCheckpoints([FromBody] DeleteCheckpointDto dto)
        {
            if (dto == null || dto.PkId <= 0)
                return BadRequest("Invalid data.");

            var result = await _auditChecklistService.DeleteSelectedCheckpointsAndTeamMembersAsync(dto);

            if (result)
                return Ok(new { message = "Deleted successfully." });

            return NotFound(new { message = "Record not found or could not be deleted." });
        }
        [HttpGet("GetCheckFinalList")]
        public async Task<IActionResult> GetChecklist(int auditId, int companyId)
        {
            var data = await _checklistService.GetChecklistAsync(auditId, companyId);
            return Ok(data);
        }

        [HttpPost("saveOrUpdate")]
        public async Task<IActionResult> SaveOrUpdateAuditSchedule([FromBody] StandardAuditScheduleDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Request data is missing.");

                // These can be retrieved from auth/user context or passed in from front-end
                int iUserID = 1; // Example user ID
                string sIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                string sModule = "Audit Module";
                string sForm = "Audit Schedule Form";
                string sEvent = dto.SA_ID > 0 ? "Update" : "Insert";
                int iMasterID = dto.SA_ID;
                string sMasterName = "StandardAuditSchedule";
                int iSubMasterID = 0;
                string sSubMasterName = "";

                string sAC = ""; // Access code if needed
                int custRegAccessCodeId = dto.SA_CompID;

                int savedId = await _auditService.InsertStandardAuditScheduleWithQuartersAsync(
                    sAC,
                    dto,
                    custRegAccessCodeId,
                    iUserID,
                    sIPAddress,
                    sModule,
                    sForm,
                    sEvent,
                    iMasterID,
                    sMasterName,
                    iSubMasterID,
                    sSubMasterName
                );

                if (savedId > 0)
                {
                    string message = dto.SA_ID > 0 ? "Updated successfully." : "Saved successfully.";
                    return Ok(new { success = true, message = message, id = savedId });
                }
                else
                {
                    return BadRequest("Failed to save the record.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error: " + ex.Message });
            }
        }
        [HttpPost("saveOrUpdateChecklist")]
        public async Task<IActionResult> SaveUpdateAuditChecklistDetails([FromBody] StandardAuditChecklistDetailsDto objSACLD)
        {
            try
            {
                var result = await _service.SaveUpdateStandardAuditChecklistDetailsAsync(objSACLD);
                var updateOrSave = result[0];
                var operId = result[1];

                string message = updateOrSave == "1" ? "Updated successfully" : "Saved successfully";
                return Ok(new { Message = message, Id = operId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }

        [HttpGet("GetByHeading")]
        public async Task<IActionResult> GetAssignedCheckpoints(
    [FromQuery] int compId,
    [FromQuery] int auditId,
    [FromQuery] int auditTypeId,
    [FromQuery] string heading)
        {
            try
            {
                var result = await _checklistService.LoadAuditTypeCheckListAsync(compId, auditId, auditTypeId, heading);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error occurred", error = ex.Message });
            }
        }
        [HttpGet("assigned-checkpoints-Assigned")]
        public async Task<IActionResult> GetAssignedCheckpoints(int auditId, int custId, string heading)
        {
            try
            {
                var checkpoints = await _auditService.GetAssignedCheckpointsAsync(auditId, custId, heading);
                return Ok(checkpoints);
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("SaveOrUpdateCustomerMaster")]
        public async Task<IActionResult> SaveCustomerMasterAsync([FromQuery] int iCompId, [FromBody] AuditCustomerDetailsDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }

            try
            {
                bool isUpdate = dto.CUST_ID > 0;

                var result = await _CustomerSaveService.SaveCustomerMasterAsync(iCompId, dto);

                string successMessage = isUpdate
                    ? "Customer master successfully updated."
                    : "Customer master successfully created.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
        [HttpPost("SaveOrUpdateFullAuditSchedule")]
        public async Task<IActionResult> SaveOrUpdateFullAuditSchedule([FromBody] StandardAuditScheduleDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                int auditScheduleId = await _auditScheduleService.SaveOrUpdateFullAuditSchedule(dto);
                return Ok(new
                {
                    Success = true,
                    Message = "Audit schedule saved successfully.",
                    AuditScheduleId = auditScheduleId
                });
            }
            catch (Exception ex)
            {
                // Log the error here if needed
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while saving the audit schedule.",
                    Error = ex.Message
                });
            }

        }
        [HttpGet("details")]
        public async Task<IActionResult> GetCustomerDetails(int iACID, int iCustId)
        {
            var result = await _service.GetCustomerDetailsAsync(iACID, iCustId);
            return Ok(result);
        }

        [HttpGet("Load-IndustryType")]
        public async Task<IActionResult> GetGeneralMasters([FromQuery] int iACID, [FromQuery] string sType)
        {
            var result = await _service.LoadGeneralMastersAsync(iACID, sType);
            return Ok(result);
        }

        [HttpGet("GetAuditStatus")]
        public async Task<IActionResult> GetAuditStatus(int saId, int companyId)
        {
            var status = await _auditService.GetAuditStatusAsync(saId, companyId);
            if (status == null)
                return NotFound("Audit status not found.");

            return Ok(status);
        }
        [HttpGet("LOEIsApproved")]
        public async Task<IActionResult> IsCustomerLoeApproved([FromQuery] int customerId, [FromQuery] int yearId)
        {
            var isApproved = await _auditService.IsCustomerLoeApprovedAsync(customerId, yearId);
            return Ok(isApproved);
        }

        [HttpPost("CheckScheduleQuarter")]
        public async Task<IActionResult> CheckScheduleQuarter([FromBody] ScheduleQuarterCheckDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            try
            {
                bool exists = await _auditService.CheckScheduleQuarterDetailsAsync(dto);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                // Log ex
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("GetLOESignedOn")]
        public IActionResult GetLOESignedOn(int compid, int auditTypeId, int customerId, int yearId)
        {
            try
            {
                var result = _auditService.GetLOESignedOnAsync(compid, auditTypeId, customerId, yearId);
                return Ok(new { LOESignedOn = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetLOEStatus")]
        public IActionResult GetLOEStatus(int compid, int auditTypeId, int customerId, int yearId)
        {
            try
            {
                var result = _auditService.GetLOEStatusAsync(compid, auditTypeId, customerId, yearId);
                return Ok(new { LOEStatus = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("date-range")]
        public async Task<IActionResult> GetScheduleQuarterDateDetails(
       [FromQuery] int iAcID,
       [FromQuery] int iAuditID,
       [FromQuery] int iQuarterID)
        {
            var (startDate, endDate) = await _quarterService.GetScheduleQuarterDateDetailsAsync(iAcID, iAuditID, iQuarterID);

            if (startDate == null || endDate == null)
                return NotFound("No records found for the given parameters.");

            return Ok(new
            {
                StartDate = startDate,
                EndDate = endDate
            });
        }
        [HttpGet("Client-Load")]
        public async Task<IActionResult> GetCustomerMaster([FromQuery] int iACID, [FromQuery] int iCustId)
        {
            var customer = await _auditService.LoadCustomerMasterAsync(iACID, iCustId);

            if (customer == null)
                return NotFound("Customer not found.");

            return Ok(customer);
        }
        [HttpPost("AuditAssistants-User-Save")]
        public async Task<IActionResult> SaveEmployee([FromBody] EmployeeDto employee)
        {
            try
            {
                var result = await _auditService.SaveEmployeeDetailsAsync(employee);

                string message = result[0] switch
                {
                    "2" => "Employee details updated successfully.",
                    "3" => "Employee details saved successfully.",
                    _ => "Operation completed."
                };

                return Ok(new
                {
                    Message = message,
                    UpdateOrSave = result[0],
                    Oper = result[1]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error saving employee", Details = ex.Message });
            }
        }

        [HttpGet("employee-details")]
        public async Task<IActionResult> GetEmployeeDetails(
       [FromQuery] int companyId,
       [FromQuery] int userId)
        {
            try
            {
                var dataTable = await _userService.LoadExistingEmployeeDetailsAsync(companyId, userId);

                if (dataTable == null)
                    return NotFound("Employee not found.");

                return Ok(dataTable);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error loading employee details", Details = ex.Message });
            }
        }

        [HttpGet("Employee-roles")]
        public async Task<IActionResult> GetActiveRoles([FromQuery] int companyId)
        {
            try
            {
                var roles = await _userService.LoadActiveRoleAsync(companyId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving roles", Details = ex.Message });
            }
        }
        [HttpGet("users-Reviewers-partner")]
        public async Task<IActionResult> GetUsersByCompanyAndRole([FromQuery] int companyId, [FromQuery] int usrRole)
        {
            var users = await _auditService.GetUsersByCompanyAndRoleAsync(companyId, usrRole);
            return Ok(users);
        }

        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GenerateCustomPdf([FromBody] AuditReportRequestDto request)
        {
            if (request.AuditId == 0)
                return BadRequest("Select Audit No.");

            var dt = await _DashboardAndScheduleInterface.LoadAuditScheduleIntervalAsync(request.AccessCodeID, request.AuditId, request.Format);
            var dt1 = await _DashboardAndScheduleInterface.LoadAssignedCheckPointsAndTeamMembersAsync(request.AccessCodeID, request.AuditId, request.CustomerId, "", request.Format);
            var dt2 = await _DashboardAndScheduleInterface.GetFinalAuditTypeHeadingsAsync(request.AccessCodeID, request.AuditId);
            var Engpartner = await _DashboardAndScheduleInterface.GetUserNamesAsync(request.AccessCodeID, request.EngagementPartnerIds);
            var Reviewer = await _DashboardAndScheduleInterface.GetUserNames1Async(request.AccessCodeID, request.EngagementQualityReviewer);
            var partner = await _DashboardAndScheduleInterface.GetUserNames2Async(request.AccessCodeID, request.Partner);
            var Assist = await _DashboardAndScheduleInterface.GetUserNames3Async(request.AccessCodeID, request.AuditAssistants);
            var pdfBytes = GeneratePdf(dt, dt1, dt2, Engpartner, Reviewer, partner, Assist, request);

            return File(pdfBytes, "application/pdf", "EmployeeAssignment.pdf");
        }
        //public byte[] GenerateExcel(DataTable dt, DataTable dt1, DataTable dt2, string Engpartner, string Reviewer, string partner, string Assist, AuditReportRequestDto request)
        //{
        //    using var workbook = new XLWorkbook();

        //    // General Info
        //    var ws = workbook.Worksheets.Add("Audit Report");
        //    int row = 1;

        //    ws.Cell(row++, 1).Value = "Audit Report";
        //    ws.Range("A1:C1").Merge().Style.Font.SetBold().Font.FontSize = 16;
        //    row++;

        //    ws.Cell(row++, 1).Value = "General Information";
        //    ws.Cell(row++, 1).Value = $"Scope Of Audit:";
        //    ws.Cell(row - 1, 2).Value = request.ScopeOfAudit;
        //    ws.Cell(row++, 1).Value = $"Customer Name:";
        //    ws.Cell(row - 1, 2).Value = request.CustomerName;
        //    ws.Cell(row++, 1).Value = $"Partner:";
        //    ws.Cell(row - 1, 2).Value = partner;
        //    ws.Cell(row++, 1).Value = $"Reviewer:";
        //    ws.Cell(row - 1, 2).Value = Reviewer;
        //    ws.Cell(row++, 1).Value = $"Audit Assistants:";
        //    ws.Cell(row - 1, 2).Value = Assist;

        //    row += 2;
        //    ws.Cell(row++, 1).Value = "Schedule Intervals";
        //    var scheduleStartRow = row;

        //    // Schedule Table Header
        //    ws.Cell(row, 1).Value = "From Date";
        //    ws.Cell(row, 2).Value = "To Date";
        //    ws.Cell(row, 3).Value = "Description";
        //    ws.Range(row, 1, row, 3).Style.Font.SetBold();
        //    row++;

        //    // Fill dt (Schedule Intervals)
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        ws.Cell(row, 1).Value = dr["FromDate"]?.ToString();
        //        ws.Cell(row, 2).Value = dr["ToDate"]?.ToString();
        //        ws.Cell(row, 3).Value = dr["Description"]?.ToString();
        //        row++;
        //    }

        //    row += 2;
        //    ws.Cell(row++, 1).Value = "Team Assignments";
        //    var teamStartRow = row;

        //    ws.Cell(row, 1).Value = "Heading";
        //    ws.Cell(row, 2).Value = "Checkpoints";
        //    ws.Cell(row, 3).Value = "Hours";
        //    ws.Cell(row, 4).Value = "Deadline";
        //    ws.Range(row, 1, row, 4).Style.Font.SetBold();
        //    row++;

        //    foreach (DataRow dr in dt1.Rows)
        //    {
        //        ws.Cell(row, 1).Value = dr["SACD_Heading"]?.ToString();
        //        ws.Cell(row, 2).Value = dr["NoCheckpoints"]?.ToString();
        //        ws.Cell(row, 3).Value = dr["Working_Hours"]?.ToString();
        //        ws.Cell(row, 4).Value = dr["Timeline"]?.ToString();
        //        row++;
        //    }

        //    row += 2;
        //    ws.Cell(row++, 1).Value = "Final Checklist";
        //    var checklistStartRow = row;

        //    ws.Cell(row, 1).Value = "Heading";
        //    ws.Cell(row, 2).Value = "Checkpoint";
        //    ws.Cell(row, 3).Value = "Mandatory";
        //    ws.Range(row, 1, row, 3).Style.Font.SetBold();
        //    row++;

        //    foreach (DataRow dr in dt2.Rows)
        //    {
        //        ws.Cell(row, 1).Value = dr["ACM_Heading"]?.ToString();
        //        ws.Cell(row, 2).Value = dr["ACM_Checkpoint"]?.ToString();
        //        ws.Cell(row, 3).Value = dr["SAC_Mandatory"]?.ToString();
        //        row++;
        //    }

        //    // Auto-fit columns
        //    ws.Columns().AdjustToContents();

        //    // Export to byte array
        //    using var stream = new MemoryStream();
        //    workbook.SaveAs(stream);
        //    return stream.ToArray();
        //}

        private byte[] GeneratePdf(
     DataTable dt,
     DataTable dt1,
     DataTable dt2,
     string Engpartner,
     string Reviewer,
     string partner,
     string Assist,
     AuditReportRequestDto request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request), "Request data is null.");

                if (dt1 == null || dt2 == null)
                    throw new ArgumentException("Required DataTables are null.");

                string Clean(string input) =>
                    string.IsNullOrWhiteSpace(input)
                        ? "N/A"
                        : input.Replace("\r", " ").Replace("\n", " ").Trim();

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        page.Content().PaddingVertical(10).Column(col =>
                        {
                            col.Item().AlignCenter()
                                .Text("Audit Schedule Report")
                                .Bold().FontSize(20);

                            col.Item().AlignCenter()
                                .LineHorizontal(1)
                                .LineColor(Colors.Black);

                            col.Item().Text(t =>
                            {
                                t.Span("Scope Of Audit: ").SemiBold();
                                t.Span(Clean(request.ScopeOfAudit));
                            });

                            col.Item().Text(t =>
                            {
                                t.Span("Customer Name: ").SemiBold();
                                t.Span(Clean(request.CustomerName));
                            });

                            col.Item().PaddingTop(5)
                                .Text("Audit Team").Bold().FontSize(14);

                            col.Item().Text(t =>
                            {
                                t.Span("Partner: ").SemiBold();
                                t.Span(Clean(partner));
                            });

                            col.Item().Text(t =>
                            {
                                t.Span("Reviewer: ").SemiBold();
                                t.Span(Clean(Reviewer));
                            });

                            col.Item().Text(t =>
                            {
                                t.Span("Audit Assistants: ").SemiBold();
                                t.Span(Clean(Assist));
                            });

                            col.Item().PaddingVertical(10)
                                .LineHorizontal(0.5f)
                                .LineColor(Colors.Grey.Medium);

                            col.Item().Text("Heading")
                                .Bold().FontSize(14).Underline();

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.ConstantColumn(80);
                                    c.ConstantColumn(60);
                                    c.ConstantColumn(100);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Element(HeaderCellStyle).Text("Heading");
                                    h.Cell().Element(HeaderCellStyle).AlignCenter().Text("Checkpoints");
                                    h.Cell().Element(HeaderCellStyle).AlignCenter().Text("Hours");
                                    h.Cell().Element(HeaderCellStyle).AlignCenter().Text("Deadline");
                                });

                                foreach (DataRow row in dt1.Rows)
                                {
                                    table.Cell().Element(CellStyle)
                                        .Text(Clean(row["SACD_Heading"]?.ToString()));

                                    table.Cell().Element(CellStyle)
                                        .AlignCenter()
                                        .Text(Clean(row["NoCheckpoints"]?.ToString()));

                                    table.Cell().Element(CellStyle)
                                        .AlignCenter()
                                        .Text(Clean(row["Working_Hours"]?.ToString()));

                                    table.Cell().Element(CellStyle)
                                        .AlignCenter()
                                        .Text(Clean(row["Timeline"]?.ToString()));
                                }
                            });

                            col.Item().PaddingVertical(10)
                                .LineHorizontal(0.5f)
                                .LineColor(Colors.Grey.Medium);

                            col.Item().Text("Checkpoints")
                                .Bold().FontSize(14).Underline();

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                    c.ConstantColumn(80);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Element(HeaderCellStyle).Text("Heading");
                                    h.Cell().Element(HeaderCellStyle).Text("Checkpoint");
                                    h.Cell().Element(HeaderCellStyle).AlignCenter().Text("Mandatory");
                                });

                                foreach (DataRow row in dt2.Rows)
                                {
                                    table.Cell().Element(CellStyle)
                                        .Text(Clean(row["ACM_Heading"]?.ToString()));

                                    table.Cell().Element(CellStyle)
                                        .Text(Clean(row["ACM_Checkpoint"]?.ToString()));

                                    table.Cell().Element(CellStyle)
                                        .AlignCenter()
                                        .Text(Clean(row["SAC_Mandatory"]?.ToString()));
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.Span("Generated on ").FontSize(10);
                            t.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).SemiBold();
                        });
                    });
                });

                return doc.GeneratePdf();
            }
            catch (Exception ex)
            {
                // Log exception here (ILogger recommended)
                throw new ApplicationException("Error occurred while generating Audit Schedule PDF.", ex);
            }
        }

        private static IContainer CellStyle(IContainer container) =>
            container.PaddingVertical(4)
                     .PaddingHorizontal(6)
                     .BorderBottom(0.5f)
                     .BorderColor(Colors.Grey.Lighten2);

        private static IContainer HeaderCellStyle(IContainer container) =>
            container.PaddingVertical(6)
                     .PaddingHorizontal(6)
                     .Background(Colors.Grey.Lighten3)
                     .BorderBottom(1)
                     .BorderColor(Colors.Black);

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers([FromQuery] int companyId)
        {
            var customers = await _DashboardAndScheduleInterface.GetCustomersAsync(companyId);
            return Ok(customers);
        }

        [HttpGet("get-formatted-date")]
        public IActionResult GetFormattedDate([FromQuery] string accessCode, [FromQuery] int accessCodeId)
        {
            if (string.IsNullOrWhiteSpace(accessCode) || accessCodeId <= 0)
                return BadRequest("Invalid access code or ID.");

            var formattedDate = _DashboardAndScheduleInterface.GetFormattedDate(accessCode, accessCodeId);
            return Ok(new { formattedDate });
        }



        [HttpPost("ChatBotAIResponse")]
        public async Task<IActionResult> Ask([FromBody] DiscoveryRequestDto request)
        {
            try
            {
                var result = await _DashboardAndScheduleInterface.GetAnswerAsync(request.Question);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("Auditframeworkid")]
        public async Task<IActionResult> GetLoeAuditFrameworkId(
       [FromQuery] int customerId,
       [FromQuery] int yearId,
       [FromQuery] int serviceTypeId)
        {
            var request = new LoeAuditFrameworkRequest
            {
                CustomerId = customerId,
                YearId = yearId,
                ServiceTypeId = serviceTypeId,
            };

            var response = await _service.GetLoeAuditFrameworkIdAsync(request);

            return Ok(response);
        }

        [HttpGet("get-Compstatus")]
        public async Task<IActionResult> GetAuditCompStatusAsync(int compId, int saId)
        {
         var status = await _DashboardAndScheduleInterface.GetAuditStatusAsync(compId, saId);

            return Ok(status);
        }
    }

}

