using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.Products;
using Marketplace.Domain.Products.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Products
{
    public class ProductRatingService : IProductRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductRatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(bool success, string message)> AddRatingAsync(int productId, double rating)
        {
            if (rating < 0 || rating > 5)
            {
                return (false, "Rating must be between 0 and 5.");
            }

            var repoProduct = _unitOfWork.GetRepository<Product>();
            var product = await repoProduct.AsQueryable().FirstOrDefaultAsync(p => p.Identifier == productId);

            if (product == null)
            {
                return (false, $"Product with ID {productId} not found.");
            }

            
            var totalReviews = product.ReviewsCount;
            var newAverageRating = ((product.AverageRating * totalReviews) + rating) / (totalReviews + 1);

            
            product.AverageRating = newAverageRating;
            product.ReviewsCount = totalReviews + 1;

            repoProduct.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Rating added successfully.");
        }
    }
}
