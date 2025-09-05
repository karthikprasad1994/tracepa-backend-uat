using Microsoft.AspNetCore.Mvc;
using TracePca.Interface;
using TracePca.Interface.CustomerUserMaster;

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
