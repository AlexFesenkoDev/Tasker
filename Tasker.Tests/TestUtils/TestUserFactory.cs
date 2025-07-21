using System.Security.Cryptography;
using System.Text;
using Tasker.Models;
using Tasker.Models.Enums;

namespace Tasker.Tests.TestUtils
{
    public static class TestUserFactory
    {
        public static User Create(string username = "testuser", string password = "password", UserRole role = UserRole.User)
        {
            using var hmac = new HMACSHA512();

            var user = new User
            {
                Username = username.ToLower(),
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                Email = $"{username}@example.com",
                Role = role
            };

            return user;
        }
    }
}
