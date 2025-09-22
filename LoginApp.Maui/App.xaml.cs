using LoginApp.Maui.Models;
using LoginApp.Maui.ViewsMahasiswa;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace LoginApp.Maui
{
    public partial class App : Application
    {
        public static User user;
        public static IConfiguration Configuration { get; private set; }
        public App()
        {
            
            InitializeComponent();

            Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


            MainPage = new NavigationPage(new LoginPage());

        }  


    }
}