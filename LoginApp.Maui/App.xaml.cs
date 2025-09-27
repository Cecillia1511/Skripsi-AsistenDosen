using LoginApp.Maui.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics; 

namespace LoginApp.Maui 
{
    public partial class App : Application 
    {
        public static User user; 
        public static IConfiguration Configuration { get; private set; }

        public App()
        {
            try
            {
                InitializeComponent();

                Configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App init error: {ex}");
                throw;
            }
        }
    }
}
