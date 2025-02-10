using Marketplace.BLL.Contracts.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceBack.Controllers.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllManufacturers()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync();
            return Ok(manufacturers);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetManufacturerById(int id)
        {
            try
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
                return Ok(manufacturer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateManufacturer([FromBody] string name)
        {
            try
            {
                var manufacturer = await _manufacturerService.CreateManufacturerAsync(name);
                return CreatedAtAction(nameof(GetManufacturerById), new { id = manufacturer.Identifier }, manufacturer);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManufacturer(int id, [FromBody] string name)
        {
            try
            {
                var updatedManufacturer = await _manufacturerService.UpdateManufacturerAsync(id, name);
                return Ok(updatedManufacturer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var (success, message) = await _manufacturerService.DeleteManufacturerAsync(id);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateManufacturerDetails(int id, string? country, string? website)
        {
            try
            {
                var updatedManufacturer = await _manufacturerService.UpdateManufacturerDetailsAsync(id, country, website);
                return Ok(updatedManufacturer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
