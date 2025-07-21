using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Tasker.Tests.TestUtils
{
    public static class TestAuthHelper
    {
        public static async Task<string> RegisterAndLoginAsync(HttpClient client)
        {
            var username = $"user_{Guid.NewGuid():N}";
            var password = "Test1234!";

            var register = new
            {
                username,
                email = $"{username}@test.com",
                password
            };

            await client.PostAsync("/auth/register", new StringContent(
                JsonConvert.SerializeObject(register), Encoding.UTF8, "application/json"));

            var login = new
            {
                username,
                password
            };

            var response = await client.PostAsync("/auth/login", new StringContent(
                JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            var token = JObject.Parse(json)["token"]?.ToString();

            return token!;
        }
    }
}
