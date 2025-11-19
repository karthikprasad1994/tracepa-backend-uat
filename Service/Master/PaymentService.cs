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

namespace TracePca.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient();
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

            // ✅ Step 5: Log payment creation
            await LogPaymentAsync(
                dbName,
                logType: "CreateOrder",
                orderId: null,
                paymentId: null,
                signature: null,
                amount: dto.Amount,
                currency: dto.Currency,
                receipt: dto.Receipt,
                notes: null,
                status: response.IsSuccessStatusCode ? "Success" : "Failed",
                responseText: responseText,
                bank: null,
                method: null,
                email: null, // Optional if available in DTO
                contact:null // Optional if available in DTO
            );

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
    }
}
