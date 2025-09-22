using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApp.Maui.Models;
using LoginApp.Maui.Services;

public partial class HomePageAkademikViewModel : ObservableObject
{
    private readonly IJadwalService _jadwalService;
    private readonly ITahunAkademikService _tahunService;

    public HomePageAkademikViewModel(
        IJadwalService jadwalService,
        ITahunAkademikService tahunService)
    {
        _jadwalService = jadwalService;
        _tahunService = tahunService;
    }


    // PROPERTIES
    [ObservableProperty]
    private string namaUser;

    [ObservableProperty]
    private List<JadwalOption> tahunAkademikList;

    [ObservableProperty]
    private JadwalOption selectedTahunAkademik;

    [ObservableProperty]
    private bool isTahunDropdownVisible;

    [ObservableProperty]
    private List<JadwalTampil> jadwalList;


    // COMMANDS
    [RelayCommand]
    private void ToggleTahunDropdown() =>
        IsTahunDropdownVisible = !IsTahunDropdownVisible;


    // INIT
    public async Task InitializeAsync()
    {
        await LoadTahunAkademikAsync();
    }


    // LOAD TAHUN AKADEMIK
    private async Task LoadTahunAkademikAsync()
    {
        TahunAkademikList = await _tahunService.GetDropdownAsync();
    }

    public async Task LoadJadwalAsync()
    {
        if (SelectedTahunAkademik is null) return;

        if (int.TryParse(SelectedTahunAkademik.Id, out int tahunId))
        {
            JadwalList = await _jadwalService.GetJadwalByTahunAsync(tahunId);
        }
        else
        {
            JadwalList = new List<JadwalTampil>();
        }
    }
}
