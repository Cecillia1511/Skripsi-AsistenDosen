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
            System.Diagnostics.Debug.WriteLine("Command berhasil dipanggil");
            await Shell.Current.GoToAsync("//LoginMahasiswa");
        }
        catch (Exception ex)
        {
            await ShowAlert("Navigation Error", $"Failed to navigate:\n{ex.Message}");
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
