using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tasker.Models;
using Tasker.Models.Enums;
using Tasker.Services;

namespace Tasker.Tests.Unit.Services
{
    public class TokenServiceTests
    {
        [Fact]
        public void CreateToken_ShouldContainValidClaims()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "this_is_my_test_key_123456789012345" },
                    { "Jwt:Issuer", "TestIssuer" },
                    { "Jwt:Audience", "TestAudience" }
                })
                .Build();

            var tokenService = new TokenService(config);

            var user = new User
            {
                Id = 42,
                Username = "alex",
                Role = UserRole.Admin
            };

            // Act
            var token = tokenService.CreateToken(user);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "42");
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "alex");
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            jwt.Issuer.Should().Be("TestIssuer");
            jwt.Audiences.Should().Contain("TestAudience");
        }
    }
}
