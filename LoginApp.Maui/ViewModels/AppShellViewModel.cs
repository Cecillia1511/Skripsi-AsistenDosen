using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApp.Maui; // pastikan namespace LoginPage

namespace LoginApp.Maui.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    [RelayCommand]
    public async Task LogoutAsync()
    {
        // 🔒 Clear session/token
        Preferences.Remove(nameof(App.user));
        App.user = null;

        // ✅ Ganti root page ke LoginPage langsung
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}
