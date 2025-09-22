using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace LoginApp.Maui.Services
{
    public class UserService
    {
        private readonly HttpClient _client;

        public UserService(IConfiguration configuration)
        {
            if (configuration["ApiUrl"] == null)
                throw new InvalidOperationException("ApiUrl not configured");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(configuration["ApiUrl"])
            };
        }

        public async Task<string?> GetNameByUsernameAsync(string username)
        {
            try
            {
                var endpoint = $"Users/nama?username={username}";
                Console.WriteLine($"Request URL: {_client.BaseAddress}{endpoint}");

                var response = await _client.GetAsync(endpoint);
                var rawResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Raw Response: {rawResponse}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch name");
                    return null;
                }

                using var doc = JsonDocument.Parse(rawResponse);
                var name = doc.RootElement.GetProperty("name").GetString();

                Console.WriteLine($"Name fetched: {name}");
                return name;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetNameByUsernameAsync: {ex.Message}");
                return null;
            }
        }
    }
}
