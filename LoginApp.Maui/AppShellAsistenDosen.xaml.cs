using LoginApp.Maui.ViewModels;

namespace LoginApp.Maui;

public partial class AppShellAsistenDosen : Shell
{
    public AppShellAsistenDosen()
    {
        InitializeComponent();
        BindingContext = new AppShellViewModel();
    }
}

