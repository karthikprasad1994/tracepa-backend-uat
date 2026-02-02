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

            // ✅ Step 2: Get Razorpay credentials
            string keyId = _configuration["Razorpay:KeyId"];
            string keySecret = _configuration["Razorpay:KeySecret"];
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyId}:{keySecret}"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // ✅ Step 3: Prepare request
            var body = new
            {
                amount = (int)(dto.Amount * 100), // amount in paise
                currency = dto.Currency ?? "INR",
                receipt = dto.Receipt ?? $"rcpt_{Guid.NewGuid()}",
                notes = dto.Notes
            };

            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // ✅ Step 4: Call Razorpay API
            var response = await _httpClient.PostAsync("https://api.razorpay.com/v1/orders", content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to create order: {responseText}");

            dynamic order = JsonConvert.DeserializeObject(responseText);
            return new
            {
                orderId = (string)order.id,
                amount = (int)order.amount,
                currency = (string)order.currency,
                key = keyId
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

            // ================= FREE TRIAL =================
            if (req.IsFreeTrial)
            {
                DateTime trialStart = DateTime.Now;
                DateTime trialEnd = trialStart.AddDays(14);

                await InsertPaymentAsync(
                    connection,
                    req,
                    status: "SUCCESS",
                    failureReason: null,
                    subscriptionStart: trialStart,
                    subscriptionEnd: trialEnd,
                    paymentGateway: "FREE"
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

            return true;
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
