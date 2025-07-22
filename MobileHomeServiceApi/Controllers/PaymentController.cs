using MHS.Service.DTOs;
using MHS.Service.Implementations;
using MHS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe.V2;

namespace MobileHomeServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnpayService;
        private readonly IStripeService _stripeService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IVNPayService vnpayService, IStripeService stripeService, ILogger<PaymentController> logger)
        {
            _vnpayService = vnpayService;
            _stripeService = stripeService;
            _logger = logger;
        }

        [HttpPost("vnpay/create")]
        public IActionResult CreateVNPayPayment([FromBody] VNPayRequest request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                request.IpAddress = ipAddress;

                var result = _vnpayService.CreatePaymentUrl(request);
                
                if (result.Success)
                {
                    return Ok(new PaymentResponse
                    {
                        Success = true,
                        PaymentUrl = result.PaymentUrl,
                        TransactionId = result.TransactionId,
                        Provider = "VNPay",
                        Message = result.Message
                    });
                }
                
                return BadRequest(new PaymentResponse
                {
                    Success = false,
                    Message = result.Message,
                    Provider = "VNPay"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment");
                return StatusCode(500, new PaymentResponse
                {
                    Success = false,
                    Message = "Internal server error",
                    Provider = "VNPay"
                });
            }
        }


        [HttpPost("vnpay/confirm")]
        public async Task<IActionResult> ConfirmVNPayPayment([FromBody] VNPayCallbackRequest callback, [FromQuery] int customerId, [FromQuery] int bookingId)
        {
            try
            {
                var result = await _vnpayService.ConfirmPaymentAndDeductBalance(callback, customerId, bookingId);
                
                if (result.IsSucceeded)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming VNPay payment");
                return StatusCode(500, new { 
                    IsSucceeded = false,
                    Message = "Internal server error occurred while processing VNPay payment confirmation"
                });
            }
        }

        [HttpPost("stripe/create")]
        public async Task<IActionResult> CreateStripePayment([FromBody] StripePaymentRequest request)
        {
            try
            {
                var result = await _stripeService.CreatePaymentIntent(request.Amount, request.Currency, request.Description);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe payment");
                return StatusCode(500, new PaymentResponse
                {
                    Success = false,
                    Message = "Internal server error",
                    Provider = "Stripe"
                });
            }
        }
        [HttpPost("stripe/confirm")]
        public async Task<IActionResult> ConfirmPayment([FromQuery] string paymentIntentId, [FromQuery] int customerId, [FromQuery] int bookingId)
        {
            try
            {
                var result = await _stripeService.ConfirmPaymentAndDeductBalance(paymentIntentId, customerId, bookingId);
                
                if (result.IsSucceeded)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming Stripe payment");
                return StatusCode(500, new { 
                    IsSucceeded = false,
                    Message = "Internal server error occurred while processing Stripe payment confirmation"
                });
            }
        }


    }
} 