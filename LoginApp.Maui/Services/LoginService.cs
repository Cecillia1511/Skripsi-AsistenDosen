using LoginApp.Maui.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LoginApp.Maui.Services;

public class LoginService : ILoginRepository
{
    public async Task<User> Login(string username, string password)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler);
            var apiUrl = Properties.Resources.ApiUrl + "Users/login";

            // 🧪 Logging input
            Console.WriteLine($"🔗 API URL: {apiUrl}");
            Console.WriteLine($"🧪 Username Sent: '{username}'");
            Console.WriteLine($"🧪 Password Sent: '{password}'");

            var loginData = new { Username = username, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            Console.WriteLine($"📦 JSON Sent: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(new Uri(apiUrl), content);

            // 🔍 Ambil isi response sebagai string
            var rawResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"📨 Raw Response: {rawResponse}");
            Console.WriteLine($"📊 Status Code: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Login failed: API returned non-success status");
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(options);

            if (loginResponse?.StatusCode == 200)
            {
                Console.WriteLine($"✅ Login success: {loginResponse.Data?.Username}");
                return loginResponse.Data;
            }

            Console.WriteLine($"⚠️ Login failed: {loginResponse?.StatusMessage}");
            await Shell.Current.DisplayAlert("Login Failed", loginResponse?.StatusMessage ?? "Unknown error", "OK");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 Exception: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            return null;
        }
    }
}
