using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using LoginApp.Maui.Services;
using LoginApp.Maui.ViewsMahasiswa;
using LoginApp.Maui.Models; // kalau kamu punya model Mahasiswa
using Microsoft.Maui.ApplicationModel; // Untuk MainThread
using Microsoft.Maui.Controls;



namespace LoginApp.Maui.ViewsMahasiswa
{
    public partial class LoginMahasiswa : ContentPage
    {
        public LoginMahasiswa()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var gmail = gmailEntry.Text?.Trim();
            var password = passwordEntry.Text;

            if (string.IsNullOrWhiteSpace(gmail) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Gmail dan password wajib diisi", "OK");
                return;
            }

            if (!gmail.EndsWith("@stu.untar.ac.id") && !gmail.EndsWith("@untar.ac.id"))
            {
                await DisplayAlert("Gmail Tidak Valid", "Gunakan Gmail resmi UNTAR untuk login", "OK");
                return;
            }

            try
            {
                var result = await AuthService.LoginMahasiswaAsync(gmail, password);

                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    // Simpan data login
                    Preferences.Set("token", result.Token);
                    Preferences.Set("gmail_mahasiswa", gmail);
                    Preferences.Set("role", "mahasiswa");

                    Application.Current.MainPage = new NavigationPage(new MahasiswaHomePage());


                }
                else
                {
                    await DisplayAlert("Login Gagal", "Gmail atau password salah", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Terjadi kesalahan: {ex.Message}", "OK");
            }
        }

        private void OnSignUpClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new RegisterMahasiswa());
        }

        private void BackClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
