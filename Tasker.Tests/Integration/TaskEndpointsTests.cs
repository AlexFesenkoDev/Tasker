using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Tasker.Models.Dtos;
using Tasker.Tests.TestUtils;

namespace Tasker.Tests.Integration
{
    public class TaskEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TaskEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTasks_WithoutAuth_ShouldReturnUnauthorized()
        {
            var response = await _client.GetAsync("/api/tasks");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Register_And_Login_ShouldReturnToken()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "password",
                Email = "t@t.com"
            };

            var regResponse = await _client.PostAsJsonAsync("/auth/register", registerDto);
            regResponse.EnsureSuccessStatusCode();

            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
            loginContent!.token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateTask_ShouldReturnCreated_WhenValidData()
        {
            // Arrange
            var token = await TestAuthHelper.RegisterAndLoginAsync(_client);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newTask = new TaskItemDto
            {
                Title = "Test Task",
                Description = "Test description",
                Priority = Models.TaskPriority.High
            };

            var content = new StringContent(JsonConvert.SerializeObject(newTask), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/tasks", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("id");
        }

        [Fact]
        public async Task CreateTask_ShouldReturnBadRequest_WhenTitleIsEmpty()
        {
            // Arrange
            var token = await TestAuthHelper.RegisterAndLoginAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var invalidTask = new TaskItemDto
            {
                Title = "",
                Description = "Description",
                Priority = Models.TaskPriority.Medium
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidTask), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/tasks", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private class LoginResult
        {
            public string token { get; set; } = "";
        }
    }
}
