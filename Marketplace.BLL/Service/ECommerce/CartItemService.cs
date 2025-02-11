using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.ECommerce;
using Marketplace.Domain.ECommerce;
using Marketplace.Domain.Products.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.ECommerce
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(bool success, string message)> AddItemToCartAsync(int cartId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return (false, "Quantity must be greater than 0.");
            }

            var repoCartItem = _unitOfWork.GetRepository<CartItem>();
            var repoProduct = _unitOfWork.GetRepository<Product>();

            
            var product = await repoProduct.AsQueryable().FirstOrDefaultAsync(p => p.Identifier == productId);
            if (product == null)
            {
                return (false, $"Product with ID {productId} not found.");
            }

            
            var existingCartItem = await repoCartItem.AsQueryable()
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (existingCartItem != null)
            {
                
                existingCartItem.Quantity += quantity;
                repoCartItem.Update(existingCartItem);
            }
            else
            {
                
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                repoCartItem.Create(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();
            return (true, "Item added to cart successfully.");
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId)
        {
            var repoCartItem = _unitOfWork.GetRepository<CartItem>();
            return await repoCartItem.AsQueryable()
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        public async Task<(bool success, string message)> RemoveItemFromCartAsync(int cartId, int productId)
        {
            var repoCartItem = _unitOfWork.GetRepository<CartItem>();

            var cartItem = await repoCartItem.AsQueryable()
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (cartItem == null)
            {
                return (false, "Item not found in cart.");
            }

            repoCartItem.Delete(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Item removed from cart successfully.");
        }

        public async Task<(bool success, string message)> UpdateItemQuantityAsync(int cartId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return (false, "Quantity must be greater than 0.");
            }

            var repoCartItem = _unitOfWork.GetRepository<CartItem>();

            var cartItem = await repoCartItem.AsQueryable()
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (cartItem == null)
            {
                return (false, "Item not found in cart.");
            }

            cartItem.Quantity = quantity;
            repoCartItem.Update(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Item quantity updated successfully.");
        }
    }
}
