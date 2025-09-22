using System.ComponentModel;
using System.Windows.Input;

public class LoginMahasiswaViewModel : INotifyPropertyChanged
{
    public string Gmail { get; set; }
    public string Password { get; set; }

    public ICommand SignInMhs { get; }

    public LoginMahasiswaViewModel()
    {
        SignInMhs = new Command(OnLogin);
    }

    public INavigation Navigation { get; set; }


    private async void OnLogin()
    {
        // Validasi input
        if (string.IsNullOrWhiteSpace(Gmail) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Gmail dan Password wajib diisi", "OK");
            return;
        }

        // Kirim ke API (nanti kita buat)
        var loginService = new LoginService();
        var result = await loginService.Login(Gmail, Password);

        await Shell.Current.GoToAsync("LoginMahasiswa");

    }

    public event PropertyChangedEventHandler PropertyChanged;
}
