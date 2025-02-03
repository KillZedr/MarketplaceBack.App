using Marketplace.BLL.Contracts.Identity;
using Marketplace.BLL.DTOs;
using Marketplace.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
                return BadRequest("Invalid user data");

            var result = await _userService.CreateUserAsync(userCreateDto);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetUserById), new { id = userCreateDto.Email }, "User created successfully");
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }


        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (userUpdateDto == null)
                return BadRequest("Invalid user data");

            var result = await _userService.UpdateUserAsync(userUpdateDto, id);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User updated successfully");
        }


            [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User deleted successfully" });
        }
    }
}
