using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApp.Maui.Models;
using LoginApp.Maui.ViewsKoordinator;
using LoginApp.Maui.ViewsMahasiswa;
using LoginApp.Maui.ViewsAsistenDosen;
using LoginApp.Maui.ViewsAkademik;
using LoginApp.Maui.ViewsKaprodi;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;

namespace LoginApp.Maui.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    private readonly ILoginRepository loginService = new LoginService();

    [RelayCommand]
    public async Task SignInAsync()
    {
        if (!IsNetworkAvailable())
        {
            await ShowAlert("Network Error", "No internet connection");
            return;
        }

        if (!AreCredentialsValid())
        {
            await ShowAlert("Error", "All fields are required");
            return;
        }

        try
        {
            var user = await loginService.Login(Username, Password);

            if (user == null)
            {
                await ShowAlert("Login Failed", "Username or Password is Incorrect");
                return;
            }

            SaveUserSession(user);

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                await ShowAlert("Login Failed", "Invalid role or user data");
                return;
            }

            NavigateByRole(user.Role);
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Something went wrong:\n{ex.Message}");
        }
    }

    [RelayCommand]
    public async Task SignInMahasiswa()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Command SignInMahasiswa dipanggil");
            await Shell.Current.GoToAsync("//LoginMahasiswa");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SIGN IN MAHASISWA] Route Error: {ex.Message}");
            await ShowAlert("Navigation Error", $"Failed to navigate:\n{ex.Message}");
        }
    }

    [RelayCommand]
    public async Task SignUpAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[SIGN UP] Navigasi ke RegisterMahasiswa dimulai – coba Shell Route");

            // Coba Shell Route dulu
            await Shell.Current.GoToAsync("//RegisterMahasiswa");

            ClearCredentials();
            System.Diagnostics.Debug.WriteLine("[SIGN UP] Shell Route berhasil");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SIGN UP] Shell Route Error: {ex.Message} – Fallback ke PushAsync");

            // FALLBACK: PushAsync dengan check Navigation available
            try
            {
                if (Application.Current.MainPage.Navigation == null)
                {
                    System.Diagnostics.Debug.WriteLine("[SIGN UP] Navigation null – wrap MainPage dengan NavigationPage");
                    // Quick fix: Wrap manual (tapi ini one-time, better setup di App.xaml.cs)
                    Application.Current.MainPage = new NavigationPage(Application.Current.MainPage);
                }

                var registerPage = new RegisterMahasiswa();
                await Application.Current.MainPage.Navigation.PushAsync(registerPage);

                ClearCredentials();
                System.Diagnostics.Debug.WriteLine("[SIGN UP] Fallback PushAsync berhasil – sekarang di RegisterMahasiswa");
            }
            catch (Exception innerEx)  // FIX: Name inner exception (bukan anonymous)
            {
                System.Diagnostics.Debug.WriteLine($"[SIGN UP] Fallback PushAsync Error: {innerEx.Message}");
                await ShowAlert("Navigation Error", $"Gagal pindah ke Register: {innerEx.Message}. Cek setup app.");
            }
        }
    }

    public void ClearCredentials()
    {
        Username = string.Empty;
        Password = string.Empty;
    }

    private bool IsNetworkAvailable() =>
        Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    private bool AreCredentialsValid() =>
        !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

    private async Task ShowAlert(string title, string message) =>
        await Shell.Current.DisplayAlert(title, message, "OK");

    private void SaveUserSession(User user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        Preferences.Set(nameof(App.user), userJson);
        Preferences.Set("username", user.Username);
        App.user = user;
    }

    private async void NavigateByRole(string role)
    {
        System.Diagnostics.Debug.WriteLine($"[DEBUG] Role from server: '{role}'");
        switch (role.ToLower())
        {
            case "koordinator":
                Application.Current.MainPage = new AppShellKoordinator();
                break;
            case "asisten dosen":
                Application.Current.MainPage = new AppShellAsistenDosen();
                break;
            case "akademik":
                Application.Current.MainPage = new AppShellAkademik();
                break;
            case "kaprodi":
                Application.Current.MainPage = new AppShellKaprodi();
                break;
            case "mahasiswa":
                Application.Current.MainPage = new AppShellMahasiswa();
                break;
            default:
                await ShowAlert("Role Error", $"Unknown role: {role}");
                break;
        }
    }
}
