using FluentAssertions;
using Moq;
using System.Security.Cryptography;
using Tasker.Models;
using Tasker.Models.Dtos;
using Tasker.Models.Enums;
using Tasker.Services;
using Tasker.Services.Interfaces;
using Tasker.Tests.TestUtils;

namespace Tasker.Tests.Unit.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegisterAsync_ShouldSucceed_WhenUsernameIsFree()
        {
            // Arrange
            using var dbContext = DbContextHelper.CreateInMemoryContext();
            var mockTokenService = new Mock<ITokenService>();
            var authService = new AuthService(dbContext, mockTokenService.Object);

            var dto = new RegisterDto
            {
                Username = "TestUser",
                Password = "secret",
                Email = "user@example.com"
            };

            // Act
            var (success, error) = await authService.RegisterAsync(dto);

            // Assert
            success.Should().BeTrue();
            error.Should().BeNull();
            dbContext.Users.Count().Should().Be(1);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenUsernameTaken()
        {
            using var dbContext = DbContextHelper.CreateInMemoryContext();
            dbContext.Users.Add(TestUserFactory.Create("testuser"));
            
            dbContext.SaveChanges();

            var authService = new AuthService(dbContext, Mock.Of<ITokenService>());

            var dto = new RegisterDto
            {
                Username = "TESTUSER",
                Password = "123",
                Email = "test@x.com"
            };

            var (success, error) = await authService.RegisterAsync(dto);

            success.Should().BeFalse();
            error.Should().Be("Username already taken");
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceed_WhenCredentialsValid()
        {
            using var dbContext = DbContextHelper.CreateInMemoryContext();
            var password = "1234";

            var user = TestUserFactory.Create("test", password, UserRole.User);

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var mockTokenService = new Mock<ITokenService>();
            mockTokenService.Setup(x => x.CreateToken(It.IsAny<User>()))
                            .Returns("mocked-token");

            var authService = new AuthService(dbContext, mockTokenService.Object);

            var dto = new LoginDto { Username = "TEST", Password = password };

            var (success, token, error) = await authService.LoginAsync(dto);

            success.Should().BeTrue();
            token.Should().Be("mocked-token");
            error.Should().BeNull();
        }

        
         [Fact]
        public async Task LoginAsync_ShouldFail_WhenPasswordInvalid()
        {
            using var dbContext = DbContextHelper.CreateInMemoryContext();
            var hmac = new HMACSHA512();
            var user = TestUserFactory.Create("bob", "correct");
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var authService = new AuthService(dbContext, Mock.Of<ITokenService>());
            var dto = new LoginDto { Username = "bob", Password = "wrong" };

            var (success, token, error) = await authService.LoginAsync(dto);

            success.Should().BeFalse();
            token.Should().BeNull();
            error.Should().Be("Invalid password");
        }
        
        [Fact]
        public async Task LoginAsync_ShouldFail_WhenUserNotFound()
        {
            using var dbContext = DbContextHelper.CreateInMemoryContext();
            var authService = new AuthService(dbContext, Mock.Of<ITokenService>());

            var dto = new LoginDto { Username = "nouser", Password = "123" };

            var (success, token, error) = await authService.LoginAsync(dto);

            success.Should().BeFalse();
            token.Should().BeNull();
            error.Should().Be("Invalid username");
        }
         
    }
}
