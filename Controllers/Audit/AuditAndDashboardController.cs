using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
    int compId, int auditId,int AuditTypeId, int custId,string sType, string heading , string? sCheckPoints)
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
    }
}
