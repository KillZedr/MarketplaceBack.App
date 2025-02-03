using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.Identity;
using Marketplace.BLL.DTOs;
using Marketplace.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;  
        public AuthService(
            UserManager<User> userManager,
            IConfiguration configuration,
            SignInManager<User> signInManager,
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            IUserService userService)  
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userService = userService;  
        }
        public async Task<string> AuthenticateUserAsync(UserLoginDto userLoginDto)
        {
            
            var user = await _userManager.Users.AsTracking()
                .FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, userLoginDto.Password)))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Генерация JWT токена
            var token = GenerateJwtToken(user);

            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);

            return token;
        }

        public async Task<string> RegisterUserAsync(UserCreateDto userCreateDto)
        {
            var result = await _userService.CreateUserAsync(userCreateDto);

            if (!result.Succeeded)
            {
              
                throw new Exception("User registration failed");
            }


            var user = await _userManager.Users.AsTracking()
                 .FirstOrDefaultAsync(u => u.Email == userCreateDto.Email);
            if (user == null)
            {
                throw new Exception("User not found after registration");
            }

          
            var token = GenerateJwtToken(user);
            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);

            return token;
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
