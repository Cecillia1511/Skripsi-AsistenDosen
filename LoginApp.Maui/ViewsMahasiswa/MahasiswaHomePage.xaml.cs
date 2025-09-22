using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace LoginApp.Maui.ViewsMahasiswa
{
    public partial class MahasiswaHomePage : ContentPage
    {
        public MahasiswaHomePage()
        {
            InitializeComponent();
            SetWelcomeText();
        }

        private void SetWelcomeText()
        {
            var gmail = Preferences.Get("gmail_mahasiswa", null);

            if (string.IsNullOrEmpty(gmail))
            {
                welcomeLabel.Text = "Selamat datang, Mahasiswa!";
                return;
            }

            var localPart = gmail.Split('@')[0];
            var nama = localPart.Split('.')[0];
            var namaFormatted = char.ToUpper(nama[0]) + nama.Substring(1);

            welcomeLabel.Text = $"Selamat datang, {namaFormatted}!";
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            Application.Current.MainPage = new NavigationPage(new LoginMahasiswa());
        }
    }
}
