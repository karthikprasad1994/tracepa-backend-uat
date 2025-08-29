using System.Net;
using System.Net.Mail;
using TracePca.Dto.Email;
using TracePca.Interface;

namespace TracePca.Service
{
    public class EmailService: EmailInterface
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

      
           public async Task SendCommonEmailAsync(CommonEmailDto dto)

        {
            if (dto.ToEmails == null || !dto.ToEmails.Any())
                throw new ArgumentException("Recipient emails required");

            string subject = "";
            string body = "";

            switch (dto.EmailType)
            {
                case "OTP":
                    subject = "Your OTP Code";
                    body = $@"
            <html><body>
                <p>Dear User,</p>
                <p>Your OTP code for verification is: <strong>{dto.Parameters["OTP"]}</strong></p>
                <p>This OTP is valid for 10 minutes.</p>
                <p>Best Regards,<br>Your Company Support Team</p>
            </body></html>";
                    break;

                case "AuditLifecycle":
                    subject = $"Intimation mail for Nearing completion of the Audit - {dto.Parameters["AuditNo"]}";
                    body = $@"
            <p><strong>Intimation mail</strong></p>
            <p>Document Requested</p>
            <p>Greetings from TRACe PA.</p>
            <p>Audit No.: {dto.Parameters["AuditNo"]} - {dto.Parameters["AuditName"]}</p>
            <p>Comments: {dto.Parameters["Remarks"]}</p>
            <p>Please login to TRACe PA portal: <a href='https://tracepacust-user.multimedia.interactivedns.com/'>Click Here</a></p>
            <p>Thanks,<br>TRACe PA Team</p>";
                    break;

                case "DuringAudit":
                    subject = $"Audit In Progress - {dto.Parameters["AuditNo"]}";
                    body = $@"
        <p><strong>During Audit Notification</strong></p>
        <p>Audit No.: {dto.Parameters["AuditNo"]}</p>
        <p>Report Type: {dto.Parameters["ReportType"]}</p>
        <p>Requested On: {dto.Parameters["RequestedOn"]}</p>
        <p>Remarks: {dto.Parameters["Remarks"]}</p>
        <p>Please login to TRACe PA portal: 
            <a href='https://tracepacust-user.multimedia.interactivedns.com/'>Click Here</a></p>
        <p>Thanks,<br>TRACe PA Team</p>";
                    break;


                default:
                    throw new Exception("Invalid EmailType");
            }

            using var mail = new MailMessage();
            mail.From = new MailAddress("trace@mmcspl.com");

            foreach (var email in dto.ToEmails)
                mail.To.Add(email);

            if (dto.CcEmails != null)
            {
                foreach (var cc in dto.CcEmails)
                    mail.CC.Add(cc);
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("trace@mmcspl.com", "sckv brni xhdt fxsw"),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mail);
        }

    }
}


