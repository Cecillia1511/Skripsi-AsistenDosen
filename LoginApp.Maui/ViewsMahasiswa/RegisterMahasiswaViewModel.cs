using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace LoginApp.Maui.ViewsMahasiswa
{
    public class RegisterMahasiswaViewModel : BindableObject
    {
        private readonly HttpClient _httpClient = new();

        public string Gmail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public INavigation Navigation { get; set; }

        public ICommand SignUpMahasiswa => new Command(async () => await RegisterAsync());

        private async Task RegisterAsync()
        {
            // Validasi input
            if (string.IsNullOrWhiteSpace(Gmail) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                Message = "Semua field harus diisi.";
                return;
            }

            if (!Regex.IsMatch(Gmail, @"^[a-zA-Z0-9._%+-]+@stu\.untar\.ac\.id$"))
            {
                Message = "Gunakan gmail kampus yang valid.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                Message = "Password dan konfirmasi tidak cocok.";
                return;
            }

            // Buat request
            var request = new RegisterRequest
            {
                Gmail = Gmail.Trim().ToLower(),
                Password = Password
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://localhost:44356/api/Mahasiswa/register", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Message = "Registrasi berhasil!";
                    await Task.Delay(1000); // opsional, kasih waktu user lihat pesan

                    await Navigation.PushAsync(new LoginMahasiswa());
                }
                else
                {
                    var error = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                    Message = error?["message"] ?? "Registrasi gagal.";
                }
            }
            catch (Exception ex)
            {
                Message = "Gmail sudah terdaftar";
            }
        }
    }

    // DTO untuk request
    public class RegisterRequest
    {
        public string Gmail { get; set; }
        public string Password { get; set; }
    }
}
