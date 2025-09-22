using LoginApp.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoginApp.Maui.Services
{
    public class LoginServiceMahasiswa
    {
        public async Task<Mahasiswa> Login(string gmail, string password)
        {
            var client = new HttpClient();
            var loginData = new { Gmail = gmail, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:44356/api/login-mahasiswa", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Mahasiswa>();
                return result;
            }

            return null;
        }
    }
}
