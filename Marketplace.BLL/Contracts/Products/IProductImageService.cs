using Marketplace.Domain.Products.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Products
{
    public interface IProductImageService : IService
    {
        Task<IEnumerable<ProductImage>> GetAllImagesAsync(); 
        Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId); 
        Task<ProductImage> AddImageAsync(int productId, string imageUrl);
        Task<(bool success, string message)> UpdateImageAsync(int imageId, string newImageUrl);
        Task<(bool success, string message)> DeleteImageAsync(int imageId);
    }
}
