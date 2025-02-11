using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.Products;
using Marketplace.BLL.DTOs.Products;
using Marketplace.Domain.Products.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Product> CreateProductAsync(int categoryId, int manufacturerId, CreateProductDto createProduct)
        {
            var repoProduct = _unitOfWork.GetRepository<Product>();
            var repoCategory = _unitOfWork.GetRepository<Category>();
            var repoManufacturer = _unitOfWork.GetRepository<Manufacturer>();

            
            var existingProduct = await repoProduct.AsQueryable().FirstOrDefaultAsync(p => p.Name == createProduct.Name);
            if (existingProduct != null)
            {
                throw new InvalidOperationException($"Product '{createProduct.Name}' already exists.");
            }

           
            var categoryExists = await repoCategory.AsQueryable().AnyAsync(c => c.Identifier == categoryId);
            if (!categoryExists)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            
            var manufacturerExists = await repoManufacturer.AsQueryable().AnyAsync(m => m.Identifier == manufacturerId);
            if (!manufacturerExists)
            {
                throw new KeyNotFoundException($"Manufacturer with ID {manufacturerId} not found.");
            }

            var product = new Product
            {
                Name = createProduct.Name,
                Description = createProduct.Description,
                Price = createProduct.Price,
                CategoryId = categoryId,  
                ManufacturerId = manufacturerId,  
                Stock = 0,  
                AverageRating = 0.0,  
                ReviewsCount = 0,  
                DiscountPrice = createProduct.Price,  
            };
            repoProduct.Create(product);
            await _unitOfWork.SaveChangesAsync();
            return product;
        }

        public async Task<(bool success, string message)> DeleteProductAsync(int id)
        {
            var repoProduct = _unitOfWork.GetRepository<Product>();

            
            var product = await repoProduct.AsQueryable().FirstOrDefaultAsync(p => p.Identifier == id);
            if (product == null)
            {
                return (false, $"Product with ID {id} not found.");
            }

     
            repoProduct.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Product deleted successfully.");
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.GetRepository<Product>().AsQueryable().ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product>()
            .AsQueryable()
            .FirstOrDefaultAsync(p => p.Identifier == id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
            {
                throw new ArgumentException("Price range values cannot be negative.");
            }

            if (minPrice > maxPrice)
            {
                throw new ArgumentException("Min price cannot be greater than max price.");
            }

            var repoProduct = _unitOfWork.GetRepository<Product>();
            return await repoProduct.AsQueryable()
                                    .Where(p => (p.DiscountPrice) >= minPrice && (p.DiscountPrice ) <= maxPrice)
                                    .ToListAsync();
        }

        public async Task<(bool success, string message)> UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            var repoProduct = _unitOfWork.GetRepository<Product>();

           
            var existingProduct = await repoProduct.AsQueryable().FirstOrDefaultAsync(p => p.Identifier == id);
            if (existingProduct == null)
            {
                return (false, $"Product with ID {id} not found.");
            }

        
            if (!string.IsNullOrEmpty(productDto.Name) && productDto.Name != existingProduct.Name)
            {
                var nameExists = await repoProduct.AsQueryable().AnyAsync(p => p.Name == productDto.Name);
                if (nameExists)
                {
                    return (false, $"Product with the name '{productDto.Name}' already exists.");
                }

                existingProduct.Name = productDto.Name;
            }

            if (!string.IsNullOrEmpty(productDto.Description))
            {
                existingProduct.Description = productDto.Description;
            }

            
            if (productDto.Price.HasValue && productDto.Price > 0)
            {
                existingProduct.Price = productDto.Price;
            }

            
            if (productDto.Stock.HasValue)
            {
                existingProduct.Stock = productDto.Stock.Value;
            }


            if (productDto.DiscountPercentage.HasValue && productDto.DiscountPercentage.Value >= 0)
            {
                
                decimal discountAmount = (existingProduct.Price ?? 0) * productDto.DiscountPercentage.Value / 100;
                existingProduct.DiscountPrice = (existingProduct.Price ?? 0) - discountAmount;
            }


            repoProduct.Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Product updated successfully.");
        }
    }
}
