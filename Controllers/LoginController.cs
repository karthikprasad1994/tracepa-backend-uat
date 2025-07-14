using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TracePca.Data.CustomerRegistration;
using TracePca.Dto;
using TracePca.Interface;
using TracePca.Models;
using TracePca.Models.CustomerRegistration;

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


        [HttpPost("sendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpReqDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new OtpResponseDto
                {
                    StatusCode = 400,
                    Message = "Email cannot be empty."
                });
            }

            var (success, message, otpToken) = await _LoginInterface.GenerateAndSendOtpJwtAsync(request.Email);

            if (!success)
            {
                return Conflict(new OtpResponseDto
                {
                    StatusCode = 409,
                    Message = message,
                    Token = null,
                    Otp = null
                });
            }

            return Ok(new OtpResponseDto
            {
                StatusCode = 200,
                Message = message,
                Token = otpToken,
                Otp = null // You can include OTP here only if needed
            });
        }


        //[HttpPost("sendOtp")]
        //public async Task<IActionResult> SendOtp([FromBody] OtpReqDto request)
        //{
        //    if (string.IsNullOrEmpty(request.Email))
        //    {
        //        return BadRequest(new OtpResponseDto
        //        {
        //            StatusCode = 400,
        //            Message = "Email cannot be empty."
        //        });
        //    }

        //    var (token, otp) = await _LoginInterface.GenerateAndSendOtpJwtAsync(request.Email);

        //    return Ok(new OtpResponseDto
        //    {
        //        StatusCode = 200,
        //        Message = "OTP sent successfully.",
        //        Token = token,
        //        Otp = otp // Include OTP in response
        //    });
        //}






        [HttpPost("VerifyOtp")]
            public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpReqDto request)
            {
                // Extract the JWT token from Authorization header
                var token = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest(new { statuscode = 400, message = "Authorization token is missing in the header." });
                }

                // Remove "Bearer " prefix if present
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }

                if (string.IsNullOrWhiteSpace(request.Otp))
                {
                    return BadRequest(new { statuscode = 400, message = "OTP cannot be empty." });
                }

                var isValid = await _LoginInterface.VerifyOtpJwtAsync(token, request.Otp);

                if (isValid)
                {
                    return Ok(new { statuscode = 200, message = "OTP verified successfully." });
                }

                return BadRequest(new { statuscode = 400, message = "Invalid or expired OTP." });
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

        [HttpGet("Loginpermissions")]
        public async Task<IActionResult> GetUserPermissions([FromQuery] int userId, [FromQuery] int companyId)
        {
            try
            {
                var permissionsList = await _LoginInterface.GetUserPermissionsWithFormNameAsync(companyId, userId);

                if (permissionsList == null || !permissionsList.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No permissions found for the user.",
                        permissions = new List<object>()
                    });
                }

                var groupedPermissions = permissionsList
                    .GroupBy(p => p.FormName)
                    .Select(g => new
                    {
                        formName = g.Key,
                        permission = string.Join(",", g.Select(x => x.Permission).Distinct())
                    })
                    .ToList();

                return Ok(new
                {
                    statusCode = 200,
                    message = "Permissions fetched successfully.",
                    permissions = groupedPermissions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching permissions.",
                    error = ex.Message
                });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("UsersLogin")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UsrEmail) || string.IsNullOrWhiteSpace(user.UsrPassWord))
            {
                return BadRequest(new { statuscode = 400, message = "Email and password are required." });
            }

            var result = await _LoginInterface.LoginUserAsync(user.UsrEmail, user.UsrPassWord);

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

        [HttpGet("CheckAndAddAccessCodeConnectionString/{accessCode}")]
        public async Task<IActionResult> CheckAndAddAccessCodeConnectionString(string accessCode)
        {
            try
            {
                var (exists, message) = await _LoginInterface.CheckAndAddAccessCodeConnectionStringAsync(accessCode);
                return Ok(new { statusCode = 200, message = message, data = exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while checking the access code.", error = ex.Message });
            }
        }
    }
}
