using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.EmployeeMaster;
using TracePca.Interface.Audit;
using TracePca.Interface.EmployeeMaster;
using TracePca.Service.EmployeeMaster;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {

        private readonly EmployeeMasterInterface _employeemaster;
        public EmployeeMasterController(EmployeeMasterInterface EmployeeInterface)
        {
            _employeemaster = EmployeeInterface;

        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUserFullName()
        {
            var result = await _employeemaster.GetUserFullNameAsync();

            if (result == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Users = (object?)null
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Users fetched successfully",
                Users = result
            });
        }

        [HttpGet("GetEmployeeDetails")]
        public async Task<IActionResult> GetEmployeeDetails(int companyId)
        {
            try
            {
                var result = await _employeemaster.GetEmployeeDetailsAsync(companyId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = $"No employees found for company ID: {companyId}"
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Employee details fetched successfully.",
                    employeeDetails = result
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


        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var result = await _employeemaster.GetRolesAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Roles fetched successfully.",
                    roles = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching roles.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("InsertUpdateEmployee")]
        public async Task<IActionResult> SaveEmployee([FromBody] EmployeeBasicDetailsDto dto)
        {
            try
            {
                var result = await _employeemaster.SaveEmployeeBasicDetailsAsync(dto);

                return Ok(new { StatusCode = 200, Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message });
            }
        }


       


        [HttpGet("GetEmployeeById")]
       
        public async Task<IActionResult> GetEmployeeById(int userId, int companyId)
        {
           

            var employee = await _employeemaster.GetEmployeeInfoByIdAsync(userId, companyId);
            if (employee == null)
                return NotFound(new { StatusCode = 404, Message = "Employee not found." });

            return Ok(new { StatusCode = 200, Message = "Employee fetched successfully.", EmployeeInfo = employee });
        }



            // GET: api/<EmployeeMasterController>
            [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<EmployeeMasterController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EmployeeMasterController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EmployeeMasterController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmployeeMasterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
