using Microsoft.AspNetCore.Mvc;
using TracePca.Interface;
using TracePca.Interface.EmployeeMaster;

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
        // GET api/<CustomerMasterController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CustomerMasterController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CustomerMasterController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomerMasterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
