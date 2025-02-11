using Marketplace.Domain.ECommerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.ECommerce
{
    public interface ICartItemService : IService
    {
        Task<(bool success, string message)> AddItemToCartAsync(int cartId, int productId, int quantity);
        Task<(bool success, string message)> UpdateItemQuantityAsync(int cartId, int productId, int quantity);
        Task<(bool success, string message)> RemoveItemFromCartAsync(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId);
    }
}
