using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpPost("VerifyPayment")]
        public IActionResult VerifyPayment([FromBody] RazorpayVerifyDto dto)
        {
            bool isValid = _paymentService.VerifySignature(dto.OrderId, dto.PaymentId, dto.Signature);
            if (isValid)
                return Ok(new { message = "Payment verified successfully" });
            else
                return BadRequest(new { message = "Invalid payment signature" });
        }
    }

    public class RazorpayVerifyDto
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
    }
}
