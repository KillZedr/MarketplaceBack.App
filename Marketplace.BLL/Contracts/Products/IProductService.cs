using Marketplace.BLL.DTOs;
using Marketplace.Domain.Products.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Products
{
    public interface IProductService : IService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(int categoryId, int manufacturerId, CreateProductDto  createProduct);
        Task<(bool success, string message)> UpdateProductAsync(int id, ProductUpdateDto productDto);
        Task<(bool success, string message)> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}
