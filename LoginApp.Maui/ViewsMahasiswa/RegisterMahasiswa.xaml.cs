using LoginApp.Maui.ViewModels;
using Microsoft.Maui.Controls;

namespace LoginApp.Maui.ViewsMahasiswa
{
    public partial class RegisterMahasiswa : ContentPage
    {
        public RegisterMahasiswa()
        {
            InitializeComponent();
            BindingContext = new RegisterMahasiswaViewModel(Navigation); 
        }

        private async void BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); 
        }
    }
}
