using FluentAssertions;
using System.Net;

namespace Tasker.Tests.Integration
{
    public class HealthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task HealthCheck_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Healthy");
        }
    }
}
