using LoginApp.Maui.ViewModels;

namespace LoginApp.Maui;

public partial class AppShellAkademik : Shell
{
    public AppShellAkademik()
    {
        InitializeComponent();
        BindingContext = new AppShellViewModel();
    }
}

