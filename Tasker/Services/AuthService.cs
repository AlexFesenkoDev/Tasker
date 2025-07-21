using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Tasker.Data;
using Tasker.Models;
using Tasker.Models.Dtos;
using Tasker.Models.Enums;
using Tasker.Services.Interfaces;

namespace Tasker.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        public async Task<(bool IsSuccess, string? Token, string? ErrorMessage)> LoginAsync(LoginDto dto)
        {
            var normalizedUsername = dto.Username.ToLower();

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedUsername);

            if (user == null)
                return (false, null, "Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                return (false, null, "Invalid password");

            var token = _tokenService.CreateToken(user);

            return (true, token, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> RegisterAsync(RegisterDto dto)
        {
            var normalizedUsername = dto.Username.ToLower();

            if (await _dbContext.Users.AnyAsync(u => u.Username.ToLower() == normalizedUsername))
                return (false, "Username already taken");

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

            return (true, null);
        }
    }
}
