using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Products
{
    public interface IProductRatingService : IService
    {
        Task<(bool success, string message)> AddRatingAsync(int productId, double rating);
    }
}
