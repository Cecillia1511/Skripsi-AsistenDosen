using System.Text;
using Microsoft.Maui.Controls;

namespace LoginApp.Maui;

public partial class GoogleLoginPage : ContentPage
{
    public GoogleLoginPage()
    {
        InitializeComponent();
    }

    private string ExtractQueryParam(string url, string key)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query.Get(key);
    }

    private void OnNavigated(object sender, WebNavigatedEventArgs e)
    {
        var url = e.Url;

        if (url.Contains("success=true")) // misalnya backend redirect ke URL ini
        {
            var token = ExtractQueryParam(url, "token");
            var nama = ExtractQueryParam(url, "nama");

            Preferences.Set("auth_token", token);
            Preferences.Set("nama_mahasiswa", nama);

            Shell.Current.GoToAsync("//MahasiswaHomePage");
        }
    }

    private string ExtractIdToken(string fragment)
    {
        var parts = fragment.TrimStart('#').Split('&');
        foreach (var part in parts)
        {
            var kv = part.Split('=');
            if (kv[0] == "id_token")
                return kv[1];
        }
        return null;
    }

    private async void SendTokenToBackend(string idToken)
    {
        var httpClient = new HttpClient();
        var content = new StringContent($"\"{idToken}\"", Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://localhost:44536/api/mahasiswa/login-google", content);
        var result = await response.Content.ReadAsStringAsync();

        await DisplayAlert("Login Result", result, "OK");
    }
}
