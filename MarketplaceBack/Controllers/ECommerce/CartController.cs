using Marketplace.BLL.Contracts.ECommerce;
using MarketplaceBack.DTOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceBack.Controllers.ECommerce
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        
        [HttpPost("create")]
        public async Task<IActionResult> CreateCart([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                var cart = await _cartService.CreateCartAsync(userId);
                return Ok(cart); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        
        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            return Ok(cart); 
        }

        [HttpDelete("delete/{cartId}")]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            var (success, message) = await _cartService.DeleteCartAsync(cartId);
            if (success)
            {
                return Ok(message); 
            }

            return NotFound(message); 
        }

        
        [HttpPost("process-payment/{cartId}")]
        public async Task<IActionResult> ProcessPayment(int cartId, [FromBody] ProcessPaymentRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("Payment details are incomplete.");
            }

            var (success, message) = await _cartService.ProcessPaymentAsync(cartId, request.TotalAmount, request.PaymentMethod, request.TransactionId, request.UserId);
            if (success)
            {
                return Ok(message); 
            }

            return BadRequest(message); 
        }
    }
}
