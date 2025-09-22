using LoginApp.Maui.Models;
using LoginApp.Maui.ViewModels;
using LoginApp.Maui.Services;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApp.Maui.ViewsAkademik;

public partial class HomePageAkademik : ContentPage
{
    private HomePageAkademikViewModel Vm => BindingContext as HomePageAkademikViewModel;

    public HomePageAkademik()
    {
        InitializeComponent();

        // Inject services ke ViewModel
        BindingContext = new HomePageAkademikViewModel(
            new JadwalService(App.Configuration),
            new TahunAkademikService(App.Configuration)
        );
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (Vm is not null)
        {
            await Vm.InitializeAsync();
        }
    }

    private async void OnTahunSelected(object sender, SelectionChangedEventArgs e)
    {
        if (Vm is null) return;

        if (e.CurrentSelection.FirstOrDefault() is JadwalOption selected)
        {
            Vm.SelectedTahunAkademik = selected;
            Vm.IsTahunDropdownVisible = false;

            await Vm.LoadJadwalAsync();
        }
    }
}
