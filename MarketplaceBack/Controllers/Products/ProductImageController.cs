using Marketplace.BLL.Contracts.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceBack.Controllers.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _productImageService;

        public ProductImageController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _productImageService.GetAllImagesAsync();
            return Ok(images);
        }

        
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetImagesByProductId(int productId)
        {
            try
            {
                var images = await _productImageService.GetImagesByProductIdAsync(productId);
                return Ok(images);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpPost("add")]
        public async Task<IActionResult> AddImage(int productId, [FromBody] string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return BadRequest(new { message = "The image URL cannot be empty." });
            }

            try
            {
                var newImage = await _productImageService.AddImageAsync(productId, imageUrl);
                return CreatedAtAction(nameof(GetImagesByProductId), new { productId = productId }, newImage);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        
        [HttpPut("update/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromBody] string newImageUrl)
        {
            if (string.IsNullOrWhiteSpace(newImageUrl))
            {
                return BadRequest(new { message = "The image URL cannot be empty." });
            }

            var (success, message) = await _productImageService.UpdateImageAsync(imageId, newImageUrl);

            if (success)
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        
        [HttpDelete("delete/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var (success, message) = await _productImageService.DeleteImageAsync(imageId);

            if (success)
            {
                return Ok(new { message });
            }
            else
            {
                return NotFound(new { message });
            }
        }
    }
}
