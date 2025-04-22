using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;

using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class OtpService
{
    private readonly IConfiguration _configuration;
    // private readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _emailOtpStorage = new();

    public OtpService(IConfiguration configuration)
    {
        _configuration = configuration;

    }

    public async Task<(string Token, string Otp)> GenerateAndSendOtpJwtAsync(string email)
    {
        string token = GenerateOtpJwt(email, out string otpCode);

        // Send OTP via Email
        await SendEmailAsync(email, otpCode);

        return (token, otpCode); // Return both token and OTP
    }


    private string GenerateOtpJwt(string email, out string otpCode)
    {
        otpCode = new Random().Next(100000, 999999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(10); // OTP expires in 10 mins

        var claims = new[]
        {
            new Claim("email", email),
            new Claim("otp", otpCode),
            new Claim("expiry", expiry.ToString("o")) // ISO 8601 format
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task SendEmailAsync(string email, string otp)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Support Team", _configuration["EmailSettings:FromEmail"]));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = "Your OTP Code";
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = $@"
            <html>
            <body>
                <p>Dear User,</p>
                <p>Your OTP code for verification is: <strong>{otp}</strong></p>
                <p>This OTP is valid for 10 minutes.</p>
                <p>Best Regards,<br>Your Company Support Team</p>
            </body>
            </html>"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"],
                                  int.Parse(_configuration["EmailSettings:SmtpPort"]),
                                  SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_configuration["EmailSettings:FromEmail"],
                                       _configuration["EmailSettings:SmtpPassword"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public bool VerifyOtpJwt(string token, string enteredOtp)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // We'll manually check expiry
                IssuerSigningKey = key
            };

            var principal = handler.ValidateToken(token, validationParameters, out var securityToken);
            var jwtToken = (JwtSecurityToken)securityToken;

            var otp = jwtToken.Claims.First(c => c.Type == "otp").Value;
            var expiry = DateTime.Parse(jwtToken.Claims.First(c => c.Type == "expiry").Value);

            return otp == enteredOtp && expiry > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}