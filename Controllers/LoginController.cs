using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using TracePca.Data.CustomerRegistration;
using TracePca.Dto;
using TracePca.Dto.Authentication;
using TracePca.Dto.Email;
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
        private readonly EmailInterface _emailService;
        //private  IHttpContextAccessor _httpContextAccessor;
        public LoginController(LoginInterface LoginInterface, EmailInterface emailService)
        {
            _LoginInterface = LoginInterface;
            _emailService = emailService;
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
                // Otp = null // You can include OTP here only if needed
            });
        }




        //[HttpPost("sendOtp")]
        //public async Task<IActionResult> SendOtp([FromBody] OtpReqDto request)
        //{
        //    // Validate request, generate OTP, etc.

        //    var dto = new CommonEmailDto
        //    {
        //        ToEmails = new List<string> { request.Email },
        //        EmailType = "OTP",
        //        Parameters = new Dictionary<string, string> { { "OTP", "123456" } }
        //    };

        //    await _emailService.SendCommonEmailAsync(dto);

        //    return Ok(new { Message = "OTP sent successfully" });
        //}




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


        [HttpPost("GmailSignup")]
        public async Task<IActionResult> SignUpUserViaGoogleAsync([FromBody] GoogleAuthDto dto)
        {
            return await _LoginInterface.SignUpUserViaGoogleAsync(dto);
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



        [HttpGet("SessionInfo")]
        public IActionResult GetSessionInfo()
        {
            var sessionStart = HttpContext.Session.GetString("SessionStartTime");

            if (string.IsNullOrEmpty(sessionStart))
            {
                sessionStart = DateTime.UtcNow.ToString("o");
                HttpContext.Session.SetString("SessionStartTime", sessionStart);
            }

            var startTime = DateTime.Parse(sessionStart);
            var expiryTime = startTime.AddMinutes(90); // Same as IdleTimeout
            var remaining = expiryTime - DateTime.UtcNow;

            if (remaining.TotalSeconds <= 0)
            {
                return Ok(new
                {
                    status = 440, // Custom status to indicate session timeout
                    message = "Session expired",
                    sessionActive = false,
                    remainingSeconds = 0
                });
            }

            return Ok(new
            {
                status = 200,
                message = "Session is active",
                sessionActive = true,
                remainingSeconds = (int)remaining.TotalSeconds
            });
        }




        //[HttpPost]
        //[Route("UserLogin")]
        //public async Task<IActionResult> Login([FromBody] LoginDto user)
        //{
        //    if (user == null || string.IsNullOrWhiteSpace(user.UsrEmail) || string.IsNullOrWhiteSpace(user.UsrPassWord))
        //    {
        //        return BadRequest(new { statuscode = 400, message = "Email and password are required." });
        //    }

        //    var result = await _LoginInterface.AuthenticateUserAsync(user.UsrEmail, user.UsrPassWord);

        //    // ✅ Use strongly typed DTO instead of reflection
        //    return result.StatusCode switch
        //    {
        //        200 => Ok(result),
        //        401 => Unauthorized(result),
        //        404 => NotFound(result),
        //        _ => StatusCode(500, result)
        //    };



        //}

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



        [HttpPost]
        [Route("UsersLogin")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UsrEmail) || string.IsNullOrWhiteSpace(user.UsrPassWord))
                return BadRequest(new { statuscode = 400, message = "Email and password are required." });

            var result = await _LoginInterface.LoginUserAsync(user.UsrEmail, user.UsrPassWord);

            return result.StatusCode == 200
                ? Ok(result)
                : StatusCode(result.StatusCode, result); // 401 will come automatically if invalid
        }


        [HttpGet("GetUsersLogs")]
        public async Task<IActionResult> GetUserLoginLogs()
        {
            try
            {
                var logs = await _LoginInterface.GetUserLoginLogsAsync();

                if (logs == null || !logs.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No login logs found.",
                        logs = Enumerable.Empty<LogInfoDto>()
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Login logs fetched successfully.",
                    logs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while fetching login logs.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutDto request)
        {
            if (string.IsNullOrWhiteSpace(request.AccessToken))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Access token is required."
                });
            }

            var success = await _LoginInterface.LogoutUserAsync(request.AccessToken);

            if (!success)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Token not found or already revoked."
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "User logged out successfully."
            });
        }

        [HttpGet("GetModules")]
        public async Task<IActionResult> GetModulesByMpId([FromQuery] int mpId = 1)
        {
            try
            {
                var modules = await _LoginInterface.GetModulesByMpIdAsync(mpId);

                if (modules == null || !modules.Any())
                {
                    return NotFound(new
                    {
                        statuscode = 404,
                        message = $"No modules found for MP_ID = {mpId}"
                    });
                }

                // Map custom names
                var mappedModules = modules.Select(m =>
                {
                    string customName = m.ModuleName switch
                    {
                        "Digital Audit Office - Financial Audit" => "Account Verification",
                        "Digital Office" => "Documents",
                        "Digital Audit Office - Assignments" => "TaskManagement",
                        _ => m.ModuleName
                    };

                    return new ModuleDto
                    {
                        ProductId = m.ProductId,
                        ModuleId = m.ModuleId,
                        ModuleName = customName
                    };
                }).ToList();

                return Ok(new
                {
                    statuscode = 200,
                    message = "Modules fetched successfully.",
                    data = mappedModules
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statuscode = 500,
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }


        [HttpPut("UpdateCustomerModules")]
        public async Task<IActionResult> UpdateCustomerModulesAsync(UpdateCustomerModulesDto dto)
        {
            if (dto.CustomerId == 0)
                return BadRequest(new { statuscode = 400, message = "CustomerId is required." });

            try
            {
                await _LoginInterface.UpdateCustomerModulesAsync(dto.CustomerId, dto.ModuleIds);
                return Ok(new { statuscode = 200, message = "Modules updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statuscode = 500, message = "Internal server error", error = ex.Message });
            }
        }







        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpGet("TestSession")]
        public IActionResult TestSession()
        {
            var customerCode = HttpContext.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(customerCode))
            {
                return Ok(new { customerCode = (string)null });
            }

            return Ok(new { customerCode }); // ✅ This always returns proper JSON
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
