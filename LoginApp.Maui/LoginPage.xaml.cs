using LoginApp.Maui.ViewModels;
using LoginApp.Maui.ViewsMahasiswa;

namespace LoginApp.Maui;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginPageViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is LoginPageViewModel vm)
        {
            vm.ClearCredentials();
        }
    }

    private async void SignInMahasiswaButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is LoginPageViewModel vm)
        {
            try
            {
                await vm.SignInMahasiswa();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login gagal: {ex.Message}", "OK");
            }
        }
    }



}