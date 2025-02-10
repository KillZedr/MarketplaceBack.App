using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.Products;
using Marketplace.Domain.Products.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Marketplace.BLL.Service.Products
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductImage> AddImageAsync(int productId, string imageUrl)
        {

           
            var repoProduct = _unitOfWork.GetRepository<Product>();
            var repoProductImage = _unitOfWork.GetRepository<ProductImage>();

           
            var findProduct = await repoProduct.AsQueryable().FirstOrDefaultAsync(p => p.Identifier == productId);

            if (findProduct != null)
            {
                
                var existingImage = await repoProductImage.AsQueryable().FirstOrDefaultAsync(pi => pi.ImageUrl == imageUrl && pi.ProductId == productId);

                if (existingImage != null)
                {
                    throw new InvalidOperationException("Image with the same URL already exists for this product.");
                }

                var productImage = new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = imageUrl
                };

                repoProductImage.Create(productImage);
                await _unitOfWork.SaveChangesAsync();

                return productImage;
            }

            throw new ArgumentException("Product not found.", nameof(productId));
        }

        public async Task<(bool success, string message)> DeleteImageAsync(int imageId)
        {
            var repoImage = _unitOfWork.GetRepository<ProductImage>();
            var findImage = repoImage.AsQueryable().FirstOrDefaultAsync(pi => pi.Identifier == imageId).Result;
            if (findImage == null)
            {
                return (false, "Image not found");
            }

            repoImage.Delete(findImage);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Image deleted successfully");
        }

        public async Task<IEnumerable<ProductImage>> GetAllImagesAsync()
        {
            var repoImage = await _unitOfWork.GetRepository<ProductImage>().AsQueryable().ToListAsync();
            return repoImage;
        }

        public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId)
        {
            var repoImade = _unitOfWork.GetRepository<ProductImage>();
            var findImages = await repoImade.AsQueryable().Where(pi => pi.ProductId == productId).ToListAsync();
            if (findImages == null || findImages.Count == 0)
            {
                throw new KeyNotFoundException("No images found for the given product ID.");
            }
            return findImages;
        }

        public async Task<(bool success, string message)> UpdateImageAsync(int imageId, string newImageUrl)
        {
            var repoProductImage = _unitOfWork.GetRepository<ProductImage>();

            
            var image = await repoProductImage.AsQueryable().FirstOrDefaultAsync(pi => pi.Identifier == imageId);

            
            if (image == null)
            {
                return (false, "Image not found.");
            }

            
            var existingImage = await repoProductImage.AsQueryable()
                                                       .FirstOrDefaultAsync(pi => pi.ProductId == image.ProductId && pi.ImageUrl == newImageUrl);

            if (existingImage != null)
            {
                return (false, "An image with this URL already exists for the product.");
            }

            
            image.ImageUrl = newImageUrl;

            repoProductImage.Update(image);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Image URL updated successfully.");
        }
    }
}
