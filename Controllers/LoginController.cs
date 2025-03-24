using Microsoft.AspNetCore.Mvc;
using TracePca.Interface;
using TracePca.Models.CustomerRegistration;
using TracePca.Models;
using TracePca.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private LoginInterface _LoginInterface;
        //private  IHttpContextAccessor _httpContextAccessor;
        public LoginController(LoginInterface LoginInterface)
        {
            _LoginInterface = LoginInterface;
            
        }
        // GET: api/<LoginController>
        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _LoginInterface.GetAllUsersAsync();
            return Ok(result); 
        }

       

        // POST api/<LoginController>
        [HttpPost]
        [Route("SignupUsers")]
        public async Task<IActionResult> AddUser([FromBody] RegistrationDto registerModel)
        {
            if (registerModel == null)
            {
                return BadRequest(new { message = "User details are required." });
            }

            var result = await _LoginInterface.SignUpUserAsync(registerModel);

            if (result is OkObjectResult okResult) // ✅ Extract from OkObjectResult
            {
                var response = okResult.Value as dynamic;
                int statusCode = response.statuscode;
                return StatusCode(statusCode, response);
            }

            return result; // Return as it is if not OkObjectResult
        }



        [HttpPost]
        [Route("UserLogin")]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UsrEmail) || string.IsNullOrWhiteSpace(user.UsrPassWord))
            {
                return BadRequest(new { statuscode = 400, message = "Email and password are required." });
            }

            var result = await _LoginInterface.AuthenticateUserAsync(user.UsrEmail, user.UsrPassWord);

            // ✅ Use strongly typed DTO instead of reflection
            return result.StatusCode switch
            {
                200 => Ok(result),
                401 => Unauthorized(result),
                404 => NotFound(result),
                _ => StatusCode(500, result)
            };
        }





        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
