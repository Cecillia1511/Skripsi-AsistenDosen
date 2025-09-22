namespace LoginApp.Maui.ViewsKoordinator;

public partial class DetailAsistenPage : ContentPage
{
    public DetailAsistenPage(AsistenDosenViewModel asisten)
    {
        InitializeComponent();
        BindingContext = asisten;
    }
}
