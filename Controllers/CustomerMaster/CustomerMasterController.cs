using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.CustomerMaster;
using TracePca.Dto.EmployeeMaster;
using TracePca.Interface;
using TracePca.Interface.EmployeeMaster;
using TracePca.Service.CustomerUserMaster;
using TracePca.Service.EmployeeMaster;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.CustomerMaster
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerMasterController : ControllerBase
    {
        private readonly CustomerMasterInterface _customermaster;

        public CustomerMasterController(CustomerMasterInterface customermaster)
        {
            _customermaster = customermaster;
        }






        // GET: api/<CustomerMasterController>
        [HttpGet("GetCustomerDetails")]

        public async Task<IActionResult> GetCustomersWithStatus(int companyId)
        {
            try
            {
                var result = await _customermaster.GetCustomersWithStatusAsync(companyId);

                return Ok(new
                {
                    status = 200,
                    message = "Customer details fetched successfully.",
                    customerDetails = result
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


        [HttpGet("GetServicesOffered")]
        public async Task<IActionResult> GetServicesOffered(int companyId)
        {
            try
            {
                var result = await _customermaster.GetServicesAsync(companyId);

                return Ok(new
                {
                    status = 200,
                    message = "Services offered fetched successfully.",
                    servicesOffered = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching services offered.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetOrganizationServices")]
        public async Task<IActionResult> GetOrganizationServices(int companyId)
        {
            try
            {
                var result = await _customermaster.GetOrganizationsAsync(companyId);

                return Ok(new
                {
                    status = 200,
                    message = "Organization services fetched successfully.",
                    organizations = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching organization services.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("GetIndustryTypes")]
        public async Task<IActionResult> GetIndustryTypes(int companyId)
        {
            try
            {
                var result = await _customermaster.GetIndustryTypesAsync(companyId);

                return Ok(new
                {
                    status = 200,
                    message = "Industry types fetched successfully.",
                    industries = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching industry types.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetManagementTypes")]
        public async Task<IActionResult> GetManagementTypes(int companyId)
        {
            try
            {
                var result = await _customermaster.GetManagementTypesAsync(companyId);

                return Ok(new
                {
                    status = 200,
                    message = "Management types fetched successfully.", 
                    managementTypes = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching management types.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("InsertUpdateCustomer")]

        public async Task<IActionResult> SaveCustomer([FromBody] CreateCustomerMasterDto dto)
        {
            try
            {
                // Call service layer
                var message = await _customermaster.SaveCustomerMasterAsync(dto);

                // Success: 200 OK
                return Ok(new { StatusCode = 200, Message = message });
            }
            catch (Exception ex)
            {
                // Handle duplicate or validation errors thrown from service
                if (ex.Message.Contains("already exists"))
                {
                    return BadRequest(new { StatusCode = 400, Message = ex.Message });
                }

                // Unexpected errors
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message });
            }
        }



        [HttpPut("UpdateStatusByCustomerId")]
        public async Task<IActionResult> ToggleCustomerStatus(int CustId)
        {
            var (isSuccess, message) = await _customermaster.ToggleCustomerStatusAsync(CustId);

            if (isSuccess)
                return Ok(new { StatusCode = 200, Message = message });

            return BadRequest(new { StatusCode = 400, Message = message });
        }



    }
}
