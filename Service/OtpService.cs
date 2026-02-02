using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using TracePca.Dto.Email;
using TracePca.Interface;

public class OtpService
{
    private readonly IConfiguration _configuration;
    private readonly EmailInterface _emailInterface;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OtpService(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        EmailInterface emailInterface)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _emailInterface = emailInterface;
    }



    // private readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _emailOtpStorage = new();

    public OtpService(IConfiguration configuration, EmailInterface emailinterface)
    {
        _configuration = configuration;
        _emailInterface = emailinterface;

    }

    public string GenerateOtpCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4]; // 4 bytes for Int32
        rng.GetBytes(bytes);

        int value = BitConverter.ToInt32(bytes, 0) & int.MaxValue; // positive int
        int otp = value % 1000000; // 6-digit

        return otp.ToString("D6");
    }


    public interface IOtpService
    {
        Task<(bool Success, string Message, int StatusCode)>
            SendForgotPasswordOtpAsync(string email);

        (bool Success, string Message)
            VerifyForgotPasswordOtp(string email, string otp);
    }


    public async Task<(bool Success, string Message, string? OtpToken, int StatusCode)> GenerateAndSendOtpJwtAsync(string email)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
        await connection.OpenAsync();

        var existingUser = await connection.QueryFirstOrDefaultAsync<string>(
            @"SELECT MCR_CustomerEmail
          FROM MMCS_CustomerRegistration 
          WHERE LOWER(MCR_CustomerEmail) = LOWER(@Email)",
            new { Email = email });

        if (existingUser != null)
        {
            return (false, "User already exists with this email.", null, 409); // 409 Conflict
        }

        // Generate OTP and JWT
        string otpCode;
        string otpJwt = GenerateOtpJwt(email, out otpCode);

        // Prepare email DTO
        var emailDto = new CommonEmailDto
        {
            ToEmails = new List<string> { email },
            EmailType = "OTP",
            Parameters = new Dictionary<string, string>
        {
            { "OTP", otpCode }
        }
        };

        // Send the OTP email using the new common method
        await _emailInterface.SendCommonEmailAsync(emailDto);

        return (true, "OTP sent successfully. Your OTP will expire in 10 minutes.", otpJwt, 200); // 200 OK
    }


    public async Task<(bool Success, string Message, string? OtpToken, int StatusCode)> ForgetPassSendOtpJwtAsync(string email)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
        await connection.OpenAsync();

        var existingUser = await connection.QueryFirstOrDefaultAsync<string>(
            @"SELECT MCR_CustomerEmail
          FROM MMCS_CustomerRegistration 
          WHERE ',' + LOWER(MCR_emails) + ',' 
            LIKE '%,' + LOWER(@Email) + ',%'",
            new { Email = email });

        if (existingUser != null)
        {
            string otpCode;
            string otpJwt = GenerateOtpJwt(email, out otpCode);

            // Prepare email DTO
            var emailDto = new CommonEmailDto
            {
                ToEmails = new List<string> { email },
                EmailType = "OTP",
                Parameters = new Dictionary<string, string>
        {
            { "OTP", otpCode }
        }
            };

            // Send the OTP email using the new common method
            await _emailInterface.SendCommonEmailAsync(emailDto);

            return (true, "OTP sent successfully. Your OTP will expire in 10 minutes.", otpJwt, 200); // 200 OK
        }
        else
        {
            return (false, "User not exists with this email.", null, 409); // 409 Conflict

        }

        // Generate OTP and JWT

    }
    public async Task<(bool Success, string Message, int StatusCode)>
       SendForgotPasswordOtpAsync(string email)
    {
        using var connection = new SqlConnection(
            _configuration.GetConnectionString("CustomerRegistrationConnection"));

        await connection.OpenAsync();

        var existingUser = await connection.QueryFirstOrDefaultAsync<string>(
            @"SELECT MCR_CustomerEmail
              FROM MMCS_CustomerRegistration
              WHERE ',' + LOWER(MCR_emails) + ',' 
            LIKE '%,' + LOWER(@Email) + ',%'",
            new { Email = email });

        if (existingUser == null)
            return (false, "User not exists with this email.", 409);

        // 🔹 Generate OTP
        string otp = new Random().Next(100000, 999999).ToString();
        DateTime expiry = DateTime.UtcNow.AddMinutes(10);

        // 🔹 Store in SESSION
        var session = _httpContextAccessor.HttpContext!.Session;
        session.SetString("FORGOT_OTP", otp);
        session.SetString("FORGOT_OTP_EMAIL", email);
        session.SetString("FORGOT_OTP_EXPIRY", expiry.ToString("O"));

        // 🔹 Send Email
        var emailDto = new CommonEmailDto
        {
            ToEmails = new List<string> { email },
            EmailType = "OTP",
            Parameters = new Dictionary<string, string>
            {
                { "OTP", otp }
            }
        };

        await _emailInterface.SendCommonEmailAsync(emailDto);

        return (
            true,
            "OTP sent successfully. Your OTP will expire in 10 minutes.",
            200
        );
    }

    // ============================
    // VERIFY OTP (SESSION BASED)
    // ============================
    public (bool Success, string Message)
        VerifyForgotPasswordOtp(string email, string otp)
    {
        var session = _httpContextAccessor.HttpContext!.Session;

        var storedOtp = session.GetString("FORGOT_OTP");
        var storedEmail = session.GetString("FORGOT_OTP_EMAIL");
        var expiryStr = session.GetString("FORGOT_OTP_EXPIRY");

        if (string.IsNullOrEmpty(storedOtp) ||
            string.IsNullOrEmpty(storedEmail) ||
            string.IsNullOrEmpty(expiryStr))
        {
            return (false, "OTP session expired.");
        }

        if (!storedEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
            return (false, "Invalid email.");

        var expiry = DateTime.Parse(expiryStr);

        if (expiry < DateTime.UtcNow)
            return (false, "OTP expired.");

        if (storedOtp != otp)
            return (false, "Invalid OTP.");

        // 🔹 Clear OTP after success
        session.Remove("FORGOT_OTP");
        session.Remove("FORGOT_OTP_EMAIL");
        session.Remove("FORGOT_OTP_EXPIRY");

        return (true, "OTP validated successfully.");
    }

    //public async Task<(bool Success, string Message, string? OtpToken)> GenerateAndSendOtpJwtAsync(string email)
    //{
    //    using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
    //    await connection.OpenAsync();

    //    var existingUser = await connection.QueryFirstOrDefaultAsync<string>(
    //        @"SELECT MCR_CustomerEmail FROM MMCS_CustomerRegistration WHERE LOWER(MCR_CustomerEmail) = LOWER(@Email)",
    //        new { Email = email });

    //    if (existingUser != null)
    //    {
    //        return (false, "User already exists with this email.", null);
    //    }

    //    string otpCode;
    //    string otpJwt = GenerateOtpJwt(email, out otpCode);

    //    // Send email to user here
    //    await SendEmailAsync(email, otpCode);

    //    return (true, "OTP sent successfully.", otpJwt);
    //}




    //public async Task<(string Token, string Otp)> GenerateAndSendOtpJwtAsync(string email)
    //{
    //    string token = GenerateOtpJwt(email, out string otpCode);

    //    // Send OTP via Email
    //    await SendEmailAsync(email, otpCode);

    //    return (token, otpCode); // Return both token and OTP
    //}


    private string GenerateOtpJwt(string email, out string otpCode)
    {
        otpCode = GenerateOtpCode();

        // otpCode = new Random().Next(100000, 999999).ToString();
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
                ValidateLifetime = true, // We'll manually check expiry
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