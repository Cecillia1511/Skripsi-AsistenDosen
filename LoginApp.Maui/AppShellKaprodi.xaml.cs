using LoginApp.Maui.ViewModels;

namespace LoginApp.Maui;

public partial class AppShellKaprodi : Shell
{
    public AppShellKaprodi()
    {
        InitializeComponent();
        BindingContext = new AppShellViewModel();
    }
}

