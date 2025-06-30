using Dapper;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Text;
using TracePca.Controllers.ProfileSetting;
using TracePca.Interface.ProfileSetting;
using static TracePca.Dto.ProfileSetting.ReportanIssueDto;

namespace TracePca.Service.ProfileSetting
{
    public class ReportanIssueService : ReportanIssueInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ReportanIssueService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;

            _env = env ?? throw new ArgumentNullException(nameof(env));


        }

        public async Task<bool> ReportIssueAsync(IssueReportDto issueDto, string userFullName, string userLogin, string accessCode)
        {
            try
            {
                // 1. Validate input
                if (string.IsNullOrEmpty(issueDto.Base64Image))
                    return false;

                // 2. Extract and decode base64 image
                string cleanBase64 = issueDto.Base64Image.Replace("data:image/png;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(cleanBase64);

                // 3. Build folder and file path
                string screenshotsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Screenshots");
                if (!Directory.Exists(screenshotsFolder))
                    Directory.CreateDirectory(screenshotsFolder);

                string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string imagePath = Path.Combine(screenshotsFolder, fileName);
                await File.WriteAllBytesAsync(imagePath, imageBytes);

                // 4. Build email body
                string subject = $"Issue Raised by {userFullName} - {DateTime.Now:dd-MMM-yyyy hh:mm tt}";
                var body = $@"
            <html>
            <body>
                <p>Dear Support Team,</p>
                <p>An issue was raised by <strong>{userFullName}</strong>.</p>
                <p>{issueDto.EmailText}</p>
                <p><strong>Access Code:</strong> {accessCode}</p>
                <p><strong>User Name:</strong> {userLogin}</p>
                <p>Visit: <a href='https://tracelites.multimedia.interactivedns.com/'>TRACe Link</a></p>
                <p>Thanks,<br/>TRACe PA Team</p>
            </body>
            </html>";

                // 5. Send email
                using var mail = new MailMessage
                {
                    From = new MailAddress("harsha.s2700@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add("varunhallur417@gmail.com");
                mail.Attachments.Add(new Attachment(imagePath));

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("harsha.s2700@gmail.com", "edvemvlmgfkcasrp"), // ⛔️ DO NOT use real Gmail password
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Error: {ex.StatusCode} - {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return false;
            }
        }
        public async Task<UserDetailsDto> GetUserDetailsAsync(int userId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "SELECT usr_FullName, usr_LoginName FROM SAD_Userdetails WHERE usr_Id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<UserDetailsDto>(sql, new { UserId = userId });
        }

    }
}
