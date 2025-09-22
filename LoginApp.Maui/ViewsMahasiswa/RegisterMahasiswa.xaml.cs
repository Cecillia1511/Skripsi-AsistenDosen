using Microsoft.Maui.Controls;

namespace LoginApp.Maui.ViewsMahasiswa
{
    public partial class RegisterMahasiswa : ContentPage
    {
        public RegisterMahasiswa()
        {
            InitializeComponent();
            BindingContext = new RegisterMahasiswaViewModel();

            var viewModel = new RegisterMahasiswaViewModel();
            viewModel.Navigation = this.Navigation;
            BindingContext = viewModel;
        }
        private void BackClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            Application.Current.MainPage = new NavigationPage(new LoginMahasiswa());
        }
    }
}
