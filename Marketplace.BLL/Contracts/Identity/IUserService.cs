using Marketplace.BLL.DTOs.Identity;
using Marketplace.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Identity
{
    public interface IUserService : IService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(UserCreateDto userCreateDto);
        Task<IdentityResult> UpdateUserAsync(UserUpdateDto userUpdateDto, string id);
        Task<IdentityResult> DeleteUserAsync(string id);
    }
}
