using LoginApp.Maui.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace LoginApp.Maui.ViewsAsistenDosen;

public partial class HomePageAsistenDosen : ContentPage
{
    private readonly UserService _userService;

    public HomePageAsistenDosen()
    {
        InitializeComponent();

        // buat instance UserService pakai configuration dari App
        _userService = new UserService(App.Configuration);

        SetWelcomeText();
    }

    private async void SetWelcomeText()
    {
        var username = Preferences.Get("Username", null);

        if (string.IsNullOrEmpty(username))
        {
            welcomeLabel.Text = "Selamat datang, Asisten Dosen!";
            return;
        }

        try
        {
            var name = await _userService.GetNameByUsernameAsync(username);

            if (!string.IsNullOrEmpty(name))
            {
                welcomeLabel.Text = $"Selamat datang, {name}!";
            }
            else
            {
                welcomeLabel.Text = "Selamat datang, Asisten Dosen!";
            }
        }
        catch
        {
            welcomeLabel.Text = "Selamat datang, Asisten Dosen!";
        }
    }
}
