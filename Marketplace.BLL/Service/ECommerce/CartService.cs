using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.ECommerce;
using Marketplace.BLL.Service.Identity;
using Marketplace.Domain.ECommerce;
using Marketplace.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.ECommerce
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
  
        private readonly UserManager<User> _userManager;

        public CartService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Cart> CreateCartAsync(string userId)
        {
            var repoCart = _unitOfWork.GetRepository<Cart>();

            var existingCart = await repoCart.AsQueryable()
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsPaid);

            if (existingCart != null)
            {
                return existingCart;
            }

            var newCart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>(),
                IsPaid = false
            };

            repoCart.Create(newCart);
            await _unitOfWork.SaveChangesAsync();
            return newCart;
        }

        public async Task<(bool success, string message)> DeleteCartAsync(int cartId)
        {
            var repoCart = _unitOfWork.GetRepository<Cart>();

            var cart = await repoCart.AsQueryable().FirstOrDefaultAsync(c => c.Identifier == cartId);

            if (cart == null)
            {
                return (false, "Cart not found.");
            }

            repoCart.Delete(cart);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Cart deleted successfully.");
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            var repoCart = _unitOfWork.GetRepository<Cart>();

            return await repoCart.AsQueryable()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsPaid);
        }

        public async Task<(bool success, string message)> ProcessPaymentAsync(int cartId, decimal totalAmount, string paymentMethod, string transactionId, string userId)
        {
            var repoCart = _unitOfWork.GetRepository<Cart>();
            var repoPaidCart = _unitOfWork.GetRepository<PaidCart>();

            // Получаем корзину по ID
            var cart = await repoCart.AsQueryable()
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Identifier == cartId);

            if (cart == null || cart.IsPaid)
            {
                return (false, "Cart not found or already paid.");
            }

            // Получаем пользователя
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            // Создаем запись о платной корзине
            var paidCart = new PaidCart
            {
                UserId = userId,
                CartId = cartId,
                TotalAmount = totalAmount,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                PaidAt = DateTime.UtcNow
            };

            // Обновляем корзину
            cart.IsPaid = true;
            cart.PaidAt = DateTime.UtcNow;
            cart.PaidCart = paidCart;

            repoCart.Update(cart);
            repoPaidCart.Create(paidCart);

            

            // Создаем новую корзину для пользователя
            var newCart = new Cart
            {
                UserId = userId,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };

            user.ActiveCart = newCart;
            repoCart.Create(newCart); // Сохраняем новую корзину
            await _unitOfWork.SaveChangesAsync();
            await _userManager.UpdateAsync(user);
            

            return (true, "Payment processed successfully and new active cart created.");
        }
    }
}
