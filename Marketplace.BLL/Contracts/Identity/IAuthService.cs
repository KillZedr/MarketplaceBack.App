using Marketplace.BLL.DTOs.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Identity
{
    public interface IAuthService : IService
    {
        Task<string> RegisterUserAsync(UserCreateDto userCreateDto);
        Task<string> AuthenticateUserAsync(UserLoginDto userLoginDto);
    }
}
