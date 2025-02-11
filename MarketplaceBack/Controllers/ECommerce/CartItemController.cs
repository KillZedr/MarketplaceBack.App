using Marketplace.BLL.Contracts.ECommerce;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceBack.Controllers.ECommerce
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        
        [HttpPost("{cartId}/items")]
        public async Task<IActionResult> AddItemToCart(int cartId, int productId, int quantity)
        {
            var result = await _cartItemService.AddItemToCartAsync(cartId, productId, quantity);

            if (result.success)
            {
                return Ok(result.message);
            }

            return BadRequest(result.message);
        }

        
        [HttpPut("{cartId}/items/{productId}")]
        public async Task<IActionResult> UpdateItemQuantity(int cartId, int productId, int quantity)
        {
            var result = await _cartItemService.UpdateItemQuantityAsync(cartId, productId, quantity);

            if (result.success)
            {
                return Ok(result.message);
            }

            return BadRequest(result.message);
        }

        
        [HttpDelete("{cartId}/items/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int cartId, int productId)
        {
            var result = await _cartItemService.RemoveItemFromCartAsync(cartId, productId);

            if (result.success)
            {
                return Ok(result.message);
            }

            return BadRequest(result.message);
        }

       
        [HttpGet("{cartId}/items")]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            var cartItems = await _cartItemService.GetCartItemsAsync(cartId);
            return Ok(cartItems);
        }
    }
}
