using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TracePca.DTOs;
using TracePca.Services;

namespace TracePca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var result = await _paymentService.CreateOrderAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Order creation failed", error = ex.Message });
            }
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
        {
            var result = await _paymentService.VerifyAndSavePaymentAsync(request);

            if (!result)
                return BadRequest(new { message = "Payment verification failed" });

            return Ok(new { message = "Payment verified successfully" });
        }
        [HttpGet("plan-version/{databaseId}")]
        public async Task<IActionResult> GetPlanVersion(long databaseId)
        {
            var planVersion = await _paymentService.GetPlanVersionAsync(databaseId);

            return Ok(new PlanVersionResponseDto
            {
                PlanVersion = planVersion
            });
        }
        [HttpGet("subscription-countdown/{databaseId}")]
        public async Task<IActionResult> GetSubscriptionCountdown(string databaseId)
        {
            var result = await _paymentService.GetSubscriptionCountdownAsync(databaseId);
            return Ok(result);
        }

    }
    public class SubscriptionCountdownDto
    {
        public string PlanVersion { get; set; }
        public int DaysLeft { get; set; }
        public bool CountdownActive { get; set; }
        public string Countdown { get; set; }   // 👈 NEW
        public string Message { get; set; }
    }

    public class PlanVersionResponseDto
    {
        public string PlanVersion { get; set; }
    }

    public class RazorpayVerifyDto
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
    }

    public class VerifyPaymentRequest
    {
        [Required]
        public string DatabaseId { get; set; }

        [Required]
        public string PlanCode { get; set; }

        [Required]
        public string PlanName { get; set; }

        [Required]
        public string BillingCycle { get; set; }

        public decimal Amount { get; set; }

        public bool IsFreeTrial { get; set; }

        // ✅ Optional
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }
    }


}
