using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("GetDropdownData")]
        public async Task<IActionResult> GetDropdownData([FromQuery] int companyId)
        {
            var result = await _AuditInterface.GetCustomerAuditDropdownAsync(companyId);
            return Ok(result);
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

        [HttpGet("LoadDRLDescription")]
        public async Task<IActionResult> LoadDRLDescription(string connectionStringName, int companyId, int drlId)
        {
            try
            {
                var result = await _AuditInterface.LoadDRLDescriptionAsync(connectionStringName, companyId, drlId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the error if needed
                return StatusCode(500, "An error occurred while retrieving the DRL description.");
            }
        }


        [HttpGet("LoadAttachments")]
        public async Task<IActionResult> LoadAttachments(string connectionStringName, int companyId, int attachId, string dateFormat = "dd/MM/yyyy")
        {
            try
            {
                var result = await _AuditInterface.LoadAttachmentsAsync(connectionStringName, companyId, attachId, dateFormat);

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






    }
}

