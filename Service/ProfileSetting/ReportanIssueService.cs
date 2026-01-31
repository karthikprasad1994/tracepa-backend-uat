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
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ReportanIssueService(IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            _env = env ?? throw new ArgumentNullException(nameof(env));


        }

        public async Task<bool> ReportIssueAsync(
        IssueReportDto issueDto,
        string userFullName,
        string userLogin,
        string accessCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(issueDto.Base64Image))
                    return false;

                // 🔹 Clean base64 (supports png/jpg/jpeg)
                var cleanBase64 = issueDto.Base64Image
                    .Replace("data:image/png;base64,", "")
                    .Replace("data:image/jpeg;base64,", "")
                    .Replace("data:image/jpg;base64,", "");

                byte[] imageBytes = Convert.FromBase64String(cleanBase64);

                // 🔹 Save screenshot
                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var screenshotsFolder = Path.Combine(rootPath, "Screenshots");

                if (!Directory.Exists(screenshotsFolder))
                    Directory.CreateDirectory(screenshotsFolder);

                var fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var imagePath = Path.Combine(screenshotsFolder, fileName);

                await File.WriteAllBytesAsync(imagePath, imageBytes);

                // 🔹 Email content
                string subject = $"Issue Raised by {userFullName} - {DateTime.Now:dd-MMM-yyyy hh:mm tt}";
                string body = $@"
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

                // 🔹 Read SMTP config
                var smtpConfig = _configuration.GetSection("Smtp");

                using var mail = new MailMessage
                {
                    From = new MailAddress(
                        smtpConfig["FromEmail"],
                        smtpConfig["FromName"]
                    ),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add("product_issues@mmcspl.com");
                mail.Attachments.Add(new Attachment(imagePath));

                using var smtp = new SmtpClient
                {
                    Host = smtpConfig["Host"],
                    Port = int.Parse(smtpConfig["Port"]),
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        smtpConfig["User"],
                        smtpConfig["Password"]
                    ),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Error: {ex.StatusCode} - {ex.Message}");
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
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT usr_FullName, usr_LoginName FROM SAD_Userdetails WHERE usr_Id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<UserDetailsDto>(sql, new { UserId = userId });
        }

    }
}
