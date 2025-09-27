using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Xaml;

namespace LoginApp.Maui.Platforms.Windows
{
    public sealed partial class App : MauiWinUIApplication
    {
        public App()
        {
            //this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
