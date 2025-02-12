using Marketplace.BLL.Contracts.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceBack.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripePaymentController : ControllerBase
    {
        private readonly IStripeService _stripeService;

        public StripePaymentController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }


        [HttpPost("create-payment-link/{idUserCart}")]
        public async Task<IActionResult> CreatePaymentLinkAsync(int idUserCart)
        {
            try
            {
                var paymentLink = await _stripeService.CreatePaymentLinkAsync(idUserCart);
                return Ok(new { PaymentLink = paymentLink });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }


        [HttpPost("cancel-payment/{paymentIntentId}")]
        public async Task<IActionResult> CancelPaymentAsync(string paymentIntentId)
        {
            try
            {

                var isRefunded = await _stripeService.RefundPaymentAsync(paymentIntentId);

                if (isRefunded)
                {
                    return Ok(new { Message = "Payment has been successfully refunded." });
                }
                else
                {
                    return BadRequest(new { Message = "Refund failed. Please try again later." });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }
    }
}
