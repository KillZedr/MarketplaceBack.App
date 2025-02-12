using Marketplace.BLL.Contracts.Payments;
using Marketplace.BLL.Settings.Stripe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace MarketplaceBack.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookStripeController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        private readonly ILogger<WebhookStripeController> _logger;
        private readonly StripeSetting _settings;

        public WebhookStripeController(IStripeService stripeService, ILogger<WebhookStripeController> logger, IOptions<StripeSetting> settings)
        {
            _stripeService = stripeService;
            _logger = logger;
            _settings = settings.Value;
        }

        
        [HttpPost("stripe-webhook")]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
            var json = await reader.ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _settings.WebhookSecret
                );

                await Task.Run(async () => await _stripeService.ProcessStripeWebhookAsync(stripeEvent));
                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error: {ex.Message}");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error while processing webhook");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
