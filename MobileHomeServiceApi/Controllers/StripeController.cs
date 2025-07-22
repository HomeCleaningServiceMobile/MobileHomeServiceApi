using MHS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MobileHomeServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly IStripeService _stripeService;

        public StripeController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }

        //    [HttpPost("create-payment-intent")]
        //    public async Task<IActionResult> CreatePaymentIntent([FromQuery] decimal amount, [FromQuery] string currency = "usd", [FromQuery] string description = "Payment")
        //    {
        //        var clientSecret = await _stripeService.CreatePaymentIntent(amount, currency, description);
        //        return Ok(new { clientSecret });
        //    }

        //    [HttpPost("confirm-payment")]
        //    public async Task<IActionResult> ConfirmPayment([FromQuery] string paymentIntentId, [FromQuery] int customerId, [FromQuery] int bookingId)
        //    {
        //        try
        //        {
        //            var result = await _stripeService.ConfirmPaymentAndDeductBalance(paymentIntentId, customerId, bookingId);
        //            if (result)
        //            {
        //                return Ok(new { message = "Payment confirmed and balance deducted successfully." });
        //            }
        //            return BadRequest(new { message = "Payment confirmation failed." });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, new { message = ex.Message });
        //        }
        //    }
        //}
    }
}
