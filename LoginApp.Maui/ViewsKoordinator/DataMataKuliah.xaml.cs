using LoginApp.Maui.Models;
using LoginApp.Maui.ViewModels;
using LoginApp.Maui.Services;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApp.Maui.ViewsKoordinator;

public partial class DataMataKuliah : ContentPage
{
    // Shortcut ke ViewModel yang sudah di-bind
    private HomePageAkademikViewModel Vm => BindingContext as HomePageAkademikViewModel;

    public DataMataKuliah()
	{
		InitializeComponent();

        // Inject service ke ViewModel dan set sebagai BindingContext
        BindingContext = new HomePageAkademikViewModel(
            new JadwalService(App.Configuration),
            new TahunAkademikService(App.Configuration)
        );
    }

    // Load data saat halaman muncul
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (Vm != null)
        {
            await Vm.InitializeAsync();
        }
    }

    // Event handler saat tahun akademik dipilih dari dropdown
    private async void OnTahunSelected(object sender, SelectionChangedEventArgs e)
    {
        if (Vm == null) return;

        if (e.CurrentSelection.FirstOrDefault() is JadwalOption selected)
        {
            Vm.SelectedTahunAkademik = selected;
            Vm.IsTahunDropdownVisible = false;

            await Vm.LoadJadwalAsync();
        }
    }

}