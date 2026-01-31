using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.ProfileSetting;
using TracePca.Service.ProfileSetting;
using static TracePca.Dto.ProfileSetting.ReportanIssueDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.ProfileSetting
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportanIssueController : ControllerBase
    {
        private ReportanIssueInterface _ReportanIssueInterface;

        public ReportanIssueController(ReportanIssueInterface ReportanIssueInterface)
        {
            _ReportanIssueInterface = ReportanIssueInterface;
        }

        [HttpPost("report-An-Issue")]
        public async Task<IActionResult> ReportIssue([FromBody] IssueReportDto dto)
        {

            var user = await _ReportanIssueInterface.GetUserDetailsAsync(dto.userid);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            string userFullName = user.usr_FullName;
            string userLogin = user.usr_LoginName;
            string accessCode = dto.accessCode; // Replace with actual access code

            bool result = await _ReportanIssueInterface.ReportIssueAsync(dto, userFullName, userLogin, accessCode);

            if (result)
                return Ok(new { success = true, message = "Issue reported successfully." });

            return StatusCode(500, new { success = false, message = "Failed to report issue." });
        }





    }
}
