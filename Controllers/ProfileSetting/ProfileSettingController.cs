using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.ProfileSetting;
using static TracePca.Dto.ProfileSetting.ProfileSettingDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.ProfileSetting
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileSettingController : ControllerBase
    {
        private ProfileSettingInterface _ProfileSettingInterface;
        private ProfileSettingInterface _ProfileSettingService;

        public ProfileSettingController(ProfileSettingInterface ProfileSettingInterface)
        {
            _ProfileSettingInterface = ProfileSettingInterface;
            _ProfileSettingService = ProfileSettingInterface;
        }

        //GetUserProfile
        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfileAsync([FromQuery] int iUserId)
        {
            try
            {
                var result = await _ProfileSettingService.GetUserProfileAsync(iUserId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //ChangePasword
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> PutChangePasswordAsync([FromQuery] string LoginName, [FromQuery] int UserId, [FromBody] TracePaChangePasswordDto dto)
        {
            try
            {
                var result = await _ProfileSettingService.PutChangePasswordAsync(LoginName, UserId, dto);

                if (result.Any() && result.First().Status == "Success")
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Password updated successfully.",
                        data = result
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "User not found or password update failed.",
                        data = result
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while updating the password.",
                    error = ex.Message
                });
            }
        }

        //GetLicenseInformation
        [HttpGet("GetLicenseInformation")]
        public async Task<IActionResult> GetLicenseInformationAsync([FromQuery] int iCustomerId, string sEmailId)
        {
            try
            {
                var result = await _ProfileSettingService.GetLicenseInformationAsync(iCustomerId, sEmailId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //UpdateUserProfile
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileAsync([FromBody] UpdateUserProfileDto dto)
        {
            try
            {
                var result = await _ProfileSettingService.UpdateUserProfileAsync(dto);

                if (result <= 0)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "User profile not found or no changes made.",
                        id = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "User profile updated successfully.",
                    id = dto.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while updating the user profile.",
                    error = ex.Message
                });
            }
        }
    }
}
