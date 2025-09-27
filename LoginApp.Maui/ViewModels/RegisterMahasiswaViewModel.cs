using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace LoginApp.Maui.ViewModels  // Sesuaikan namespace sesuai project kamu
{
    public partial class RegisterMahasiswaViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly INavigation _navigation;  // Untuk navigasi setelah sukses

        // Properti Binding (dari XAML)
        [ObservableProperty]
        private string nama = string.Empty;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        [PasswordPropertyText]  // Optional: Hide di debugger
        private string password = string.Empty;

        [ObservableProperty]
        [PasswordPropertyText]
        private string confirmPassword = string.Empty;

        // Loading State (untuk show/hide spinner di XAML, optional)
        [ObservableProperty]
        private bool isLoading = false;

        // Error Message (bind ke Label di XAML kalau mau tampil error)
        [ObservableProperty]
        private string errorMessage = string.Empty;

        public RegisterMahasiswaViewModel(INavigation navigation = null)
        {
            _navigation = navigation;
            _httpClient = new HttpClient();  // Atau inject dari DI (MauiProgram.cs)
            _httpClient.BaseAddress = new Uri("https://your-api-base-url.com/");  // Ganti dengan API URL kamu (e.g., localhost:5000)
        }

        // Command untuk Tombol Register
        [RelayCommand]
        private async Task RegisterAsync()
        {
            try
            {
                // Reset error
                ErrorMessage = string.Empty;

                // Validasi Input
                if (string.IsNullOrWhiteSpace(Nama) || string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Semua field harus diisi!";
                    await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Password dan konfirmasi password tidak cocok!";
                    await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                    return;
                }

                if (!Email.Contains("@") || !Email.EndsWith("@stu.untar.ac.id"))  // Validasi sederhana email Untar
                {
                    ErrorMessage = "Email harus berakhiran @stu.untar.ac.id!";
                    await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                    return;
                }

                if (Password.Length <= 5)
                {
                    ErrorMessage = "Password minimal 5 karakter!";
                    await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                    return;
                }

                // Set loading
                IsLoading = true;

                // Prepare data untuk POST
                var registerData = new
                {
                    Nama = Nama.Trim(),
                    Username = Username.Trim(),  // NIM/Username
                    Email = Email.Trim().ToLower(),
                    Password = Password  // Backend harus hash ini (e.g., BCrypt)
                };

                // Call API: POST ke endpoint register
                var response = await _httpClient.PostAsJsonAsync("Auth/RegisterMahasiswa", registerData);  // Sesuaikan endpoint

                if (response.IsSuccessStatusCode)
                {
                    // Parse response (asumsi JSON { success: true, message: "..." })
                    var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();  // Buat class di bawah

                    await Application.Current.MainPage.DisplayAlert("Sukses", result?.Message ?? "Akun berhasil dibuat! Silakan login.", "OK");

                    // Clear form
                    Nama = Username = Email = Password = ConfirmPassword = string.Empty;

                    // Navigasi ke LoginPage (asumsi Shell atau NavigationPage)
                    if (_navigation != null)
                    {
                        await _navigation.PopAsync();  // Kembali ke halaman sebelumnya (e.g., Login)
                    }
                    else
                    {
                        // Atau pakai Shell navigation: await Shell.Current.GoToAsync("//LoginPage");
                        await Shell.Current.GoToAsync("//LoginPage");  // Sesuaikan route
                    }
                }
                else
                {
                    // Error dari backend (e.g., username sudah ada)
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Gagal register: {response.StatusCode} - {errorContent}";
                    await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = "Koneksi gagal. Cek internet atau API URL.";
                await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                System.Diagnostics.Debug.WriteLine($"[REGISTER] HTTP Error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                ErrorMessage = "Format response API salah.";
                await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                System.Diagnostics.Debug.WriteLine($"[REGISTER] JSON Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Terjadi kesalahan tak terduga.";
                await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
                System.Diagnostics.Debug.WriteLine($"[REGISTER] Unexpected Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Helper Class untuk Response API (buat di file terpisah atau di sini)
        public class RegisterResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
