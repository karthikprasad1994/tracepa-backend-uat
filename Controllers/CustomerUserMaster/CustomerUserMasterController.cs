using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.CustomerMaster;
using TracePca.Dto.CustomerUserMaster;
using TracePca.Dto.EmployeeMaster;
using TracePca.Interface;
using TracePca.Interface.CustomerUserMaster;
using TracePca.Service.CustomerUserMaster;
using TracePca.Service.EmployeeMaster;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerUserMasterController : ControllerBase
    {
        private readonly CustomerUserMasterInterface _customerusermaster;

        public CustomerUserMasterController(CustomerUserMasterInterface customerusermaster)
        {
            _customerusermaster = customerusermaster;
        }


        [HttpGet("GetUsers")]
        public async Task<IActionResult> LoadExistingUsers(int companyId, string? search = "")
        {
            try
            {
                var result = await _customerusermaster.LoadExistingUsersAsync(companyId, search);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No users found.",
                        users = Enumerable.Empty<UserDropdownDto>() // ✅ strongly typed empty list
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Users fetched successfully.",
                    users = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching users.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetActiveCustomers")]
        public async Task<IActionResult> LoadActiveCustomers(int companyId)
        {
            try
            {
                var result = await _customerusermaster.LoadActiveCustomersAsync(companyId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No active customers found.",
                        customers = Enumerable.Empty<CustomerDropdownDto>() // ✅ Empty list
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Active customers fetched successfully.",
                    customers = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching active customers.",
                    error = ex.Message
                });
            }
        }


        // GET: api/<CustomerUserMasterController>
        [HttpGet("GetCustomerUserDetails")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int companyId)
        {
            try
            {
                if (companyId <= 0)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "Invalid CompanyId"
                    });
                }

                var result = await _customerusermaster.GetAllUserDetailsAsync(companyId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No users found."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "CustomerUsers fetched successfully.",
                    CustomerUserDetails = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = ex.Message
                });
            }
        }
        [HttpPost("InsertUpdateCustomer")]
        public async Task<IActionResult> InsertCustomer([FromBody] CreateCustomerUsersDto dto)
        {
            try
            {
                var result = await _customerusermaster.InsertCustomerUsersDetailsAsync(dto);

                return Ok(new { StatusCode = 200, Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message });
            }
        }


        // GET api/<CustomerUserMasterController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CustomerUserMasterController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CustomerUserMasterController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomerUserMasterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
