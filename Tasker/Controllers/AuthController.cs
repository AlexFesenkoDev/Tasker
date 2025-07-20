using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Tasker.Data;
using Tasker.Models;
using Tasker.Models.Dtos;
using Tasker.Models.Enums;
using Tasker.Services;

namespace Tasker.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext dbContext, TokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var normalizedUsername = dto.Username.ToLower();

            if (await _dbContext.Users.AnyAsync(u => u.Username.ToLower() == normalizedUsername))
                return BadRequest("Username already taken");

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = normalizedUsername,
                Email = dto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key,
                Role = UserRole.User
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var normalizedUsername = dto.Username.ToLower();

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedUsername);

            if (user == null)
                return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized("Invalid password");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }
    }
}
