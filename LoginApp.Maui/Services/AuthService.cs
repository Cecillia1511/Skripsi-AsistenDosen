using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoginApp.Maui.Helpers;
using LoginApp.Maui.Models;
using Microsoft.Maui.Storage;

namespace LoginApp.Maui.Services
{
    public static class AuthService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<LoginResult> LoginMahasiswaAsync(string gmail, string password)
        {
            // Validasi domain UNTAR
            if (!ValidationHelper.IsValidUntarEmail(gmail))
                return null;

            var loginData = new
            {
                Gmail = gmail,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:44356/api/Mahasiswa/login", content);

                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var jsonDoc = JsonDocument.Parse(responseBody);
                    var root = jsonDoc.RootElement;

                    // Coba ambil token kalau tersedia
                    string token = root.TryGetProperty("Token", out var tokenProp)
                        ? tokenProp.GetString()
                        : "dummy-token";

                    string gmailFromServer = root.TryGetProperty("Gmail", out var gmailProp)
                        ? gmailProp.GetString()
                        : gmail;

                    return new LoginResult
                    {
                        IsSuccess = true,
                        Token = token,
                        Gmail = gmailFromServer
                    };
                }
                else
                {
                    Console.WriteLine($"Login gagal. Status: {response.StatusCode}, Body: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }

            return null;
        }
    }
}
