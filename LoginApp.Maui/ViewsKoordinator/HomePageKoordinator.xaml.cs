using LoginApp.Maui.ViewModels;

namespace LoginApp.Maui.ViewsKoordinator
{
    public partial class HomePageKoordinator : ContentPage
    {
        public HomePageKoordinator()
        {
            InitializeComponent();
        }
        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Clear(); // kalau kamu simpan token atau data login

            var loginPage = new LoginPage();

            if (loginPage.BindingContext is LoginPageViewModel vm)
            {
                vm.ClearCredentials();
            }

            Application.Current.MainPage = new NavigationPage(loginPage);
        }

    }
}
