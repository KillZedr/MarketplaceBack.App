using Marketplace.BLL.Contracts.Products;
using Marketplace.BLL.DTOs;
using Marketplace.Domain.Products.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceBack.Controllers.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductRatingService _productRatingService;
        public ProductController(IProductService productService, IProductRatingService productRatingService)
        {
            _productService = productService;
            _productRatingService = productRatingService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving products: {ex.Message}");
            }
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProductAsync(int categoryId, int manufacturerId, [FromBody] CreateProductDto createProduct)
        {
            if (createProduct == null)
            {
                return BadRequest("Product data is required.");
            }

            try
            {
                var product = await _productService.CreateProductAsync(categoryId, manufacturerId, createProduct);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Identifier }, product);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);  
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);  
            }
        }
        [HttpPost("{id}/rating")]
        public async Task<IActionResult> AddRating(int id, [FromBody] double rating)
        {
            var result = await _productRatingService.AddRatingAsync(id, rating);

            if (result.success)
            {
                return Ok(result.message);
            }

            return BadRequest(result.message);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto productUpdate)
        {
            if (productUpdate == null)
            {
                return BadRequest("Product data is required.");
            }

            try
            {
                var result = await _productService.UpdateProductAsync(id, productUpdate);
                if (result.success)
                {
                    return Ok(result.message);  
                }

                return NotFound(result.message);  
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);  
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (result.success)
                {
                    return Ok(result.message);
                }
                else
                {
                    return NotFound(result.message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the product: {ex.Message}");
            }
        }

        
        [HttpGet("price-range")]
        public async Task<IActionResult> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
            {
                return BadRequest("Price range values cannot be negative.");
            }

            try
            {
                var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving products by price range: {ex.Message}");
            }
        }
    }
}
