using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.Identity;
using Marketplace.BLL.DTOs.Identity;
using Marketplace.Domain.ECommerce;
using Marketplace.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<User> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }
        

        public  async Task<IdentityResult> CreateUserAsync(UserCreateDto userCreateDto)
        {
    
            if (!IsValidEmail(userCreateDto.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid email format" });
            }


            if (!IsValidPassword(userCreateDto.Password))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Password must be at least 6 characters long and contain a digit or letter" });
            }


            userCreateDto.FirstName = CapitalizeFirstLetter(userCreateDto.FirstName);
            userCreateDto.LastName = CapitalizeFirstLetter(userCreateDto.LastName);
            string username = userCreateDto.FirstName.ToLower() + "." + userCreateDto.LastName.ToLower();
            var newUser = new User()
            {
                Email = userCreateDto.Email,
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                Password = userCreateDto.Password,
                UserName = username
                
            };

            var result = await _userManager.CreateAsync(newUser, userCreateDto.Password);
            if (!result.Succeeded)
            {
                return result;
            }
            var roleExist = await _roleManager.RoleExistsAsync("USER");
            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("USER"));
                if (!roleResult.Succeeded)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Failed to create role 'USER'" });
                }
            }

          
            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, "USER");
            if (!addToRoleResult.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Failed to add user to role 'USER'" });
            }
            var userCart = new Cart
            {
                UserId = newUser.Id,  
                IsPaid = false
            };
            var repoCart = _unitOfWork.GetRepository<Cart>();
            repoCart.Create(userCart);
            
            await _unitOfWork.SaveChangesAsync();

            newUser.ActiveCart = userCart;
            var updateUserResult = await _userManager.UpdateAsync(newUser);
            if (!updateUserResult.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Failed to update user with active cart" });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            var userCart = await _unitOfWork.GetRepository<Cart>().AsQueryable().FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (userCart != null)
            {
                _unitOfWork.GetRepository<Cart>().Delete(userCart);
                await _unitOfWork.SaveChangesAsync();
            }

            return await _userManager.DeleteAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .Include(u => u.ActiveCart)  
                .Include(u => u.PaidCarts)
                .Include(u => u.ActiveCart.CartItems)
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.Users
                .Include(u => u.ActiveCart) 
                .Include(u => u.PaidCarts)
                .Include(u => u.ActiveCart.CartItems)
                .FirstOrDefaultAsync(u => u.Email == email);

        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userManager.Users
                .Include(u => u.ActiveCart)  
                .Include(u => u.PaidCarts)
                .Include(u => u.ActiveCart.CartItems)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateUserAsync(UserUpdateDto user, string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (!string.IsNullOrEmpty(user.FirstName))
                existingUser.FirstName = user.FirstName;

            if (!string.IsNullOrEmpty(user.LastName))
                existingUser.LastName = user.LastName;

            if (!string.IsNullOrEmpty(user.PhoneNumber))
                existingUser.PhoneNumber = user.PhoneNumber;

            if (!string.IsNullOrEmpty(user.Address))
                existingUser.Address = user.Address;

            if (!string.IsNullOrEmpty(user.Country))
                existingUser.Country = user.Country;


            return await _userManager.UpdateAsync(existingUser);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

       
        private bool IsValidPassword(string password)
        {
            return password.Length >= 6 && password.Any(char.IsLetterOrDigit);
        }

       
        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
}
