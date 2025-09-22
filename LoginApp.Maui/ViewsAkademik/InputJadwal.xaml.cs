using LoginApp.Maui.Models;
using LoginApp.Maui.ViewModels;

namespace LoginApp.Maui.ViewsAkademik;

public partial class InputJadwal : ContentPage
{
    private InputJadwalViewModel Vm => BindingContext as InputJadwalViewModel;

    public InputJadwal()
    {
        InitializeComponent();
        BindingContext = new InputJadwalViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Vm != null)
            await Vm.LoadDropdownsAsync();
    }

    // Helper method untuk semua dropdown
    private void OnItemSelected(
        SelectionChangedEventArgs e,
        Action<JadwalOption> setSelected,
        Action setDropdownClose)
    {
        if (e.CurrentSelection.FirstOrDefault() is JadwalOption selected)
        {
            setSelected(selected);
            setDropdownClose();
        }
    }

    private void OnTahunAkademikSelected(object sender, SelectionChangedEventArgs e) =>
        OnItemSelected(e,
            selected => Vm.SelectedTahunAkademik = selected,
            () => Vm.IsTahunAkademikDropdownVisible = false);

    private void OnMataKuliahSelected(object sender, SelectionChangedEventArgs e) =>
        OnItemSelected(e,
            selected => Vm.SelectedMataKuliah = selected,
            () => Vm.IsMataKuliahDropdownVisible = false);

    private void OnDosenSelected(object sender, SelectionChangedEventArgs e) =>
        OnItemSelected(e,
            selected => Vm.SelectedDosen = selected,
            () => Vm.IsDosenDropdownVisible = false);

    private void OnHariSelected(object sender, SelectionChangedEventArgs e) =>
        OnItemSelected(e,
            selected => Vm.SelectedHari = selected,
            () => Vm.IsHariDropdownVisible = false);

    private void OnWaktuSelected(object sender, SelectionChangedEventArgs e) =>
        OnItemSelected(e,
            selected => Vm.SelectedWaktu = selected,
            () => Vm.IsWaktuDropdownVisible = false);

    private void OnRuanganSelected(object sender, SelectionChangedEventArgs e) =>
        OnItemSelected(e,
            selected => Vm.SelectedRuangan = selected,
            () => Vm.IsRuanganDropdownVisible = false);

    private void OnKelasSelected(object sender, SelectionChangedEventArgs e) =>
        OnItemSelected(e,
            selected => Vm.SelectedKelas = selected,
            () => Vm.IsKelasDropdownVisible = false);
}
