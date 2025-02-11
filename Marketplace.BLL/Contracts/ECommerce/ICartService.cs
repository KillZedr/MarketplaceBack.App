using Marketplace.Domain.ECommerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.ECommerce
{
    public interface ICartService : IService
    {
        Task<Cart> CreateCartAsync(string userId);
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<(bool success, string message)> DeleteCartAsync(int cartId);
        Task<(bool success, string message)> ProcessPaymentAsync(int cartId, decimal totalAmount, string paymentMethod, string transactionId, string userId);
    }
}
