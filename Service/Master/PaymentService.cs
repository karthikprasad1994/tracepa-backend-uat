using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TracePca.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Dapper;
using TracePca.Controllers;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Net.Mail;
using System.Net;
using TracePca.Models.CustomerRegistration;

namespace TracePca.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string _connectionString;

        public PaymentService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient();
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            _connectionString = _configuration.GetConnectionString("CustomerRegistrationConnection");
        }

        public async Task<object> CreateOrderAsync(CreateOrderDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Calculate GST (18% for Karnataka, India)
            decimal gstRate = 0.18m; // 18% GST
            decimal baseAmount = dto.Amount;
            decimal gstAmount = Math.Round(baseAmount * gstRate, 2);
            decimal totalAmount = baseAmount + gstAmount;

            // ✅ Step 3: Get Razorpay credentials
            string keyId = _configuration["Razorpay:KeyId"];
            string keySecret = _configuration["Razorpay:KeySecret"];
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyId}:{keySecret}"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // ✅ Step 4: Prepare request with GST breakdown in notes
            var notes = dto.Notes ?? new Dictionary<string, string>();

            // Add GST details to notes
            notes["gst_rate"] = "18%";
            notes["gst_amount"] = gstAmount.ToString("F2");
            notes["base_amount"] = baseAmount.ToString("F2");
            notes["total_amount"] = totalAmount.ToString("F2");
            notes["gst_state"] = "Karnataka, India";

            var body = new
            {
                amount = (int)(totalAmount * 100), // amount in paise (INCLUDING GST)
                currency = dto.Currency ?? "INR",
                receipt = dto.Receipt ?? $"rcpt_{Guid.NewGuid()}",
                notes = notes
            };

            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // ✅ Step 5: Call Razorpay API
            var response = await _httpClient.PostAsync("https://api.razorpay.com/v1/orders", content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to create order: {responseText}");

            dynamic order = JsonConvert.DeserializeObject(responseText);
            return new
            {
                orderId = (string)order.id,
                baseAmount = baseAmount,
                gstAmount = gstAmount,
                totalAmount = totalAmount,
                currency = (string)order.currency,
                key = keyId,
                gstRate = "18%",
                gstState = "Karnataka, India"
            };
        }

        public bool VerifySignature(string orderId, string paymentId, string signature)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string keySecret = _configuration["Razorpay:KeySecret"];
            string text = $"{orderId}|{paymentId}";
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(keySecret);
            var messageBytes = encoding.GetBytes(text);

            using var hmac = new HMACSHA256(keyBytes);
            string computedSignature = BitConverter.ToString(hmac.ComputeHash(messageBytes)).Replace("-", "").ToLower();
            bool isValid = computedSignature == signature;

            // ✅ Log signature verification
            _ = LogPaymentAsync(
                dbName,
                logType: "VerifySignature",
                orderId: orderId,
                paymentId: paymentId,
                signature: signature,
                amount: null,
                currency: null,
                receipt: null,
                notes: null,
                status: isValid ? "Valid" : "Invalid",
                responseText: $"ComputedSignature: {computedSignature}",
                bank: null,
                method: null,
                email: null,
                contact: null
            );

            return isValid;
        }

        // ✅ Common DB logging using dynamic connection from session
        private async Task LogPaymentAsync(
            string dbName,
            string logType,
            string orderId,
            string paymentId,
            string signature,
            decimal? amount,
            string currency,
            string receipt,
            string notes,
            string status,
            string responseText,
            string bank,
            string method,
            string email,
            string contact)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(dbName);
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = @"
                    INSERT INTO PaymentLogs 
                    (OrderId, PaymentId, Signature, Amount, Currency, Receipt, Notes, Status, LogType, ResponseText, Bank, Method, Email, Contact)
                    VALUES 
                    (@OrderId, @PaymentId, @Signature, @Amount, @Currency, @Receipt, @Notes, @Status, @LogType, @ResponseText, @Bank, @Method, @Email, @Contact)";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@OrderId", (object)orderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PaymentId", (object)paymentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Signature", (object)signature ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Amount", (object)amount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Currency", (object)currency ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Receipt", (object)receipt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LogType", logType);
                cmd.Parameters.AddWithValue("@ResponseText", (object)responseText ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Bank", (object)bank ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Method", (object)method ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Contact", (object)contact ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaymentService] Log failed: {ex.Message}");
            }
        }
        private async Task InsertPaymentAsync(
      SqlConnection connection,
      VerifyPaymentRequest req,
      string status,
      string failureReason,
      DateTime? subscriptionStart = null,
      DateTime? subscriptionEnd = null,
      string paymentGateway = null
  )
        {
            // 1️⃣ Get Database_Id (MCR_ID)

            
            const string dbIdSql = @"
        SELECT MCR_ID
        FROM dbo.MMCS_CustomerRegistration
        WHERE MCR_CustomerCode = @CustomerCode";

            int dbId = await connection.ExecuteScalarAsync<int>(
                dbIdSql,
                new { CustomerCode = req.DatabaseId }
            );

            // 2️⃣ Insert payment transaction
            const string insertSql = @"
        INSERT INTO PaymentTransactions
        (
            Database_Id,
            PlanCode,
            PlanName,
            BillingCycle,
            Amount,
            IsFreeTrial,
            TrialStartDate,
            TrialEndDate,
            PaymentGateway,
            GatewayOrderId,
            GatewayPaymentId,
            PaymentStatus,
            FailureReason,
            SubscriptionStart,
            SubscriptionEnd,
            CreatedOn
        )
        VALUES
        (
            @Database_Id,
            @PlanCode,
            @PlanName,
            @BillingCycle,
            @Amount,
            @IsFreeTrial,
            @TrialStartDate,
            @TrialEndDate,
            @PaymentGateway,
            @GatewayOrderId,
            @GatewayPaymentId,
            @PaymentStatus,
            @FailureReason,
            @SubscriptionStart,
            @SubscriptionEnd,
            GETDATE()
        )";

            await connection.ExecuteAsync(insertSql, new
            {
                Database_Id = dbId,
                req.PlanCode,
                req.PlanName,
                req.BillingCycle,
                req.Amount,
                req.IsFreeTrial,

                TrialStartDate = req.IsFreeTrial ? DateTime.UtcNow : (DateTime?)null,
                TrialEndDate = req.IsFreeTrial ? DateTime.UtcNow.AddDays(29) : (DateTime?)null,

                PaymentGateway = paymentGateway,
                GatewayOrderId = req.RazorpayOrderId,
                GatewayPaymentId = req.RazorpayPaymentId,
                PaymentStatus = status,
                FailureReason = failureReason,

                SubscriptionStart = subscriptionStart,
                SubscriptionEnd = subscriptionEnd
            });
        }


        private bool VerifyRazorpaySignature(string orderId, string paymentId, string signature)
        {
            var secret = _configuration["Razorpay:KeySecret"];

            var payload = $"{orderId}|{paymentId}";
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(secretBytes);
            var hash = hmac.ComputeHash(payloadBytes);
            var generatedSignature = Convert.ToHexString(hash).ToLower();

            return generatedSignature == signature;
        }
        public async Task<bool> VerifyAndSavePaymentAsync(VerifyPaymentRequest req)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();


            string CustName = string.Empty;
            string CustEmail = string.Empty;

            using (var cmd = new SqlCommand(@"
    SELECT 
        MCR_CustomerName,
        MCR_CustomerEmail
    FROM dbo.MMCS_CustomerRegistration
    WHERE MCR_CustomerCode = @CustomerCode
", connection))
            {
                cmd.Parameters.AddWithValue("@CustomerCode", req.DatabaseId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    CustName = reader["MCR_CustomerName"]?.ToString();
                    CustEmail = reader["MCR_CustomerEmail"]?.ToString();
                }
                else
                {
                    throw new Exception("Customer not found");
                }
            }

            // ================= FREE TRIAL =================
            if (req.IsFreeTrial)
            {
                DateTime trialStart = DateTime.Now;
                DateTime trialEnd = trialStart.AddDays(30);

                await InsertPaymentAsync(
                    connection,
                    req,
                    status: "SUCCESS",
                    failureReason: null,
                    subscriptionStart: trialStart,
                    subscriptionEnd: trialEnd,
                    paymentGateway: "FREE"
                );

                // Send free trial email to customer and notification to admin
                await SendSubscriptionEmailAsync(
                    customerEmail: CustEmail,
                    customerName: CustName,
                    planName: req.PlanName,
                    billingCycle: req.BillingCycle,
                    amount: req.Amount,
                    isFreeTrial: true,
                    trialEndDate: trialEnd,
                    subscriptionStart: trialStart,
                    subscriptionEnd: trialEnd
                );

                // Send admin notification for free trial signup
                await SendAdminNotificationEmailAsync(
                    customerEmail: CustEmail,
                    customerName: CustName,
                    planName: req.PlanName,
                    billingCycle: req.BillingCycle,
                    amount: req.Amount,
                    isFreeTrial: true,
                    isPaymentSuccessful: true
                );

                return true;
            }

            // ================= PAID PAYMENT =================
            bool isValid = VerifyRazorpaySignature(
                req.RazorpayOrderId,
                req.RazorpayPaymentId,
                req.RazorpaySignature
            );

            if (!isValid)
            {
                await InsertPaymentAsync(
                    connection,
                    req,
                    status: "FAILED",
                    failureReason: "Invalid Razorpay Signature",
                    paymentGateway: "RAZORPAY"
                );

                // Send payment failure email to customer
                await SendPaymentFailedEmailAsync(
                    customerEmail: CustEmail,
                    customerName: CustName,
                    planName: req.PlanName,
                    amount: req.Amount
                );

                // Send admin notification for failed payment
                await SendAdminNotificationEmailAsync(
                    customerEmail: CustEmail,
                    customerName: CustName,
                    planName: req.PlanName,
                    billingCycle: req.BillingCycle,
                    amount: req.Amount,
                    isFreeTrial: false,
                    isPaymentSuccessful: false
                );

                return false;
            }

            DateTime start = DateTime.Now;
            DateTime end = req.BillingCycle == "YEARLY"
                ? start.AddYears(1)
                : start.AddMonths(1);

            await InsertPaymentAsync(
                connection,
                req,
                status: "SUCCESS",
                failureReason: null,
                subscriptionStart: start,
                subscriptionEnd: end,
                paymentGateway: "RAZORPAY"
            );

            // Send successful payment email to customer
            await SendSubscriptionEmailAsync(
                customerEmail: CustEmail,
                customerName: CustName,
                planName: req.PlanName,
                billingCycle: req.BillingCycle,
                amount: req.Amount,
                isFreeTrial: false,
                subscriptionStart: start,
                subscriptionEnd: end
            );

            // Send admin notification for successful payment
            await SendAdminNotificationEmailAsync(
                customerEmail: CustEmail,
                customerName: CustName,
                planName: req.PlanName,
                billingCycle: req.BillingCycle,
                amount: req.Amount,
                isFreeTrial: false,
                isPaymentSuccessful: true
            );

            return true;
        }

        private async Task SendSubscriptionEmailAsync(
            string customerEmail,
            string customerName,
            string planName,
            string billingCycle,
            decimal amount,
            bool isFreeTrial,
            DateTime subscriptionStart,
            DateTime subscriptionEnd,
            DateTime? trialEndDate = null)
        {
            try
            {
                string subject = isFreeTrial
                    ? $"🎉 Your {planName} Free Trial is Activated!"
                    : $"✅ Payment Successful - {planName} Subscription Activated";

                string body = BuildSubscriptionEmailBody(
                    customerName: customerName,
                    planName: planName,
                    billingCycle: billingCycle,
                    amount: amount,
                    isFreeTrial: isFreeTrial,
                    subscriptionStart: subscriptionStart,
                    subscriptionEnd: subscriptionEnd,
                    trialEndDate: trialEndDate
                );

                // Send email to customer
                await SendEmailAsync(
                    fromEmail: "Trace@mmcspl.com",
                    fromName: "MMCS PVT LTD Subscription Service",
                    to: customerEmail,
                    subject: subject,
                    body: body,
                    isHtml: true
                );

            }
            catch (Exception ex)
            {
            }
        }
        private string BuildSubscriptionEmailBody(
    string customerName,
    string planName,
    string billingCycle,
    decimal amount,
    bool isFreeTrial,
    DateTime subscriptionStart,
    DateTime subscriptionEnd,
    DateTime? trialEndDate = null)
{
    if (isFreeTrial)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; }}
        .info-box {{ background-color: #f9f9f9; border-left: 4px solid #4CAF50; padding: 15px; margin: 15px 0; }}
        .footer {{ margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Free Trial Activated! 🎉</h2>
        </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>Congratulations! Your free trial for <strong>{planName}</strong> has been successfully activated.</p>
            
            <div class='info-box'>
                <h3>Trial Details:</h3>
                <p><strong>Plan:</strong> {planName}</p>
                <p><strong>Trial Start:</strong> {subscriptionStart:dd MMM yyyy}</p>
                <p><strong>Trial End:</strong> {trialEndDate:dd MMM yyyy}</p>
                <p><strong>Regular Price:</strong> ₹{amount}/{billingCycle}</p>
            </div>

            <p>Need help? Contact our support team.</p>
            <p>Best regards,<br>MMCS Team</p>
        </div>
        <div class='footer'>
            <p>This is an automated email. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";
    }
    else
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; }}
        .info-box {{ background-color: #f9f9f9; border-left: 4px solid #2196F3; padding: 15px; margin: 15px 0; }}
        .footer {{ margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Payment Successful! ✅</h2>
        </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>Thank you for your purchase! Your payment has been processed successfully.</p>
            
            <div class='info-box'>
                <h3>Subscription Details:</h3>
                <p><strong>Plan:</strong> {planName}</p>
                <p><strong>Billing Cycle:</strong> {billingCycle}</p>
                <p><strong>Amount Paid:</strong> ₹{amount}</p>
                <p><strong>Start Date:</strong> {subscriptionStart:dd MMM yyyy}</p>
                <p><strong>End Date:</strong> {subscriptionEnd:dd MMM yyyy}</p>
                <p><strong>Transaction Date:</strong> {DateTime.Now:dd MMM yyyy HH:mm}</p>
            </div>
            
            <p>You can now access all features of your {planName} plan.</p>
            <p>Need help? Contact our support team.</p>
            <p>Best regards,<br>MMCS Team</p>
        </div>
        <div class='footer'>
            <p>This is an automated email. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";
    }
}

        private async Task SendPaymentFailedEmailAsync(
            string customerEmail,
            string customerName,
            string planName,
            decimal amount)
        {
            try
            {
                string subject = "❌ Payment Failed - Action Required";
                string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f44336; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; }}
        .footer {{ margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Payment Failed</h2>
        </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>We were unable to process your payment for the <strong>{planName}</strong> plan.</p>
            <p><strong>Amount:</strong> ₹{amount}</p>
            <p>Please try again or contact our support team for assistance.</p>
            <p>Best regards,<br>MMCS Team</p>
        </div>
        <div class='footer'>
            <p>This is an automated email. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";

                // Send email to customer
                await SendEmailAsync(
                    fromEmail: "Trace@mmcspl.com",
                    fromName: "MMCS PVT LTD Subscription Service",
                    to: customerEmail,
                    subject: subject,
                    body: body,
                    isHtml: true
                );

            }
            catch (Exception ex)
            {
            }
        }

        private async Task SendAdminNotificationEmailAsync(
            string customerEmail,
            string customerName,
            string planName,
            string billingCycle,
            decimal amount,
            bool isFreeTrial,
            bool isPaymentSuccessful)
        {
            try
            {
                string subject = isFreeTrial
                    ? $"📊 New Free Trial Signup: {customerName}"
                    : isPaymentSuccessful
                        ? $"💰 New Payment Received: {customerName}"
                        : $"⚠️ Payment Failed: {customerName}";

                string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #333; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; }}
        .info-box {{ background-color: #f5f5f5; border: 1px solid #ddd; padding: 15px; margin: 15px 0; }}
        .success {{ color: #4CAF50; }}
        .failed {{ color: #f44336; }}
        .trial {{ color: #2196F3; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Subscription Notification</h2>
        </div>
        <div class='content'>
            <h3 class='{(isFreeTrial ? "trial" : isPaymentSuccessful ? "success" : "failed")}'>
                {subject}
            </h3>
            
            <div class='info-box'>
                <h4>Customer Details:</h4>
                <p><strong>Name:</strong> {customerName}</p>
                <p><strong>Email:</strong> {customerEmail}</p>
                <p><strong>Plan:</strong> {planName}</p>
                <p><strong>Billing Cycle:</strong> {billingCycle}</p>
                <p><strong>Amount:</strong> ₹{amount}</p>
                <p><strong>Type:</strong> {(isFreeTrial ? "Free Trial" : "Paid Subscription")}</p>
                <p><strong>Status:</strong> {(isPaymentSuccessful ? "SUCCESS" : "FAILED")}</p>
                <p><strong>Date:</strong> {DateTime.Now:dd MMM yyyy HH:mm}</p>
            </div>
            
            <p>This is an automated notification from MMCS PVT LTD Subscription System.</p>
        </div>
    </div>
</body>
</html>";

                // Send admin notification to varun@mmcspl.com
                await SendEmailAsync(
                    fromEmail: "Trace@mmcspl.com",
                    fromName: "MMCS PVT LTD Subscription System",
                    to: "sujatha@mmcspl.com,venu@mmcspl.com",
                    subject: subject,
                    body: body,
                    isHtml: true
                );

            }
            catch (Exception ex)
            {
            }
        }
        public async Task SendEmailAsync(
        string fromEmail,
        string fromName,
        string to,
        string subject,
        string body,
        bool isHtml = false)
        {
            try
            {
                using var client = new SmtpClient(_configuration["Smtp:Host"])
                {
                    Port = int.Parse(_configuration["Smtp:Port"]),
                    Credentials = new NetworkCredential(
                        _configuration["Smtp:User"],
                        _configuration["Smtp:Password"]
                    ),
                    EnableSsl = true // Gmail SMTP requires SSL
                };

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);

                await client.SendMailAsync(message);
            }
            catch
            {
                throw;
            }
        }


        public async Task<string> GetPlanVersionAsync(long databaseId)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
        SELECT
            CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM PaymentTransactions
                    WHERE Database_Id = @DatabaseId
                      AND IsFreeTrial = 0
                      AND PaymentStatus = 'SUCCESS'
                      AND SubscriptionStart <= GETDATE()
                      AND SubscriptionEnd >= GETDATE()
                ) THEN 'PAID'

                WHEN EXISTS (
                    SELECT 1
                    FROM PaymentTransactions
                    WHERE Database_Id = @DatabaseId
                      AND IsFreeTrial = 1
                      AND PaymentStatus = 'SUCCESS'
                      AND TrialStartDate <= GETDATE()
                      AND TrialEndDate >= GETDATE()
                ) THEN 'TRIAL'

                ELSE 'EXPIRED / NO PLAN'
            END AS PlanVersion;
        ";

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DatabaseId", databaseId);

            var result = await command.ExecuteScalarAsync();
            return result?.ToString();
        }
        public async Task<SubscriptionCountdownDto> GetSubscriptionCountdownAsync(string databaseId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string dbIdSql = @"
        SELECT MCR_ID
        FROM dbo.MMCS_CustomerRegistration
        WHERE MCR_CustomerCode = @CustomerCode";

            int dbId = await connection.ExecuteScalarAsync<int>(
            dbIdSql,
                new { CustomerCode = databaseId }
            );
            var sql = @"
    SELECT TOP 1
        IsFreeTrial,
        CASE 
            WHEN IsFreeTrial = 1 THEN TrialEndDate
            ELSE SubscriptionEnd
        END AS EndDate
    FROM PaymentTransactions
    WHERE Database_Id = @dbId
      AND PaymentStatus = 'SUCCESS'
      AND (
            (IsFreeTrial = 1 AND TrialStartDate <= GETDATE() AND TrialEndDate >= GETDATE())
            OR
            (IsFreeTrial = 0 AND SubscriptionStart <= GETDATE() AND SubscriptionEnd >= GETDATE())
          )
    ORDER BY IsFreeTrial ASC, CreatedOn DESC;
    ";

            await connection.OpenAsync();
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@dbId", dbId);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read())
            {
                return new SubscriptionCountdownDto
                {
                    PlanVersion = "EXPIRED / NO PLAN",
                    CountdownActive = false
                };
            }

            bool isFreeTrial = Convert.ToBoolean(reader["IsFreeTrial"]);
            DateTime endDate = Convert.ToDateTime(reader["EndDate"]);

            TimeSpan remaining = endDate - DateTime.Now;

            int daysLeft = (int)Math.Floor(remaining.TotalDays);

            bool countdownActive =
                (isFreeTrial && daysLeft <= 30) ||
                (!isFreeTrial && daysLeft <= 30);

            string countdown = null;
            string message = null;

            if (remaining.TotalSeconds > 0 && countdownActive)
            {
                countdown =
                    $"{remaining.Days}d:" +
                    $"{remaining.Hours:D2}h:" +
                    $"{remaining.Minutes:D2}m:" +
                    $"{remaining.Seconds:D2}s";

                message = isFreeTrial
                    ? $"Your free trial expires in {countdown}"
                    : $"Your subscription expires in {countdown}";
            }

            return new SubscriptionCountdownDto
            {
                PlanVersion = isFreeTrial ? "TRIAL" : "PAID",
                DaysLeft = daysLeft,
                CountdownActive = countdownActive,
                Countdown = countdown,
                Message = message
            };
        }

    }
}
