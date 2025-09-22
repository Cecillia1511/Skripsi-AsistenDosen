using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApp.Maui;
using LoginApp.Maui.Models;
using LoginApp.Maui.Services;
using System.Collections.ObjectModel;

public partial class InputJadwalViewModel : ObservableObject
{
    private readonly JadwalService _service;

    public InputJadwalViewModel()
    {
        _service = new JadwalService(App.Configuration);
    }


    private List<JadwalOption> _allMataKuliah, _allTahunAkademik, _allDosen,
                               _allHari, _allWaktu, _allRuangan, _allKelas;


    [ObservableProperty] private ObservableCollection<JadwalOption> filteredMataKuliahList;
    [ObservableProperty] private ObservableCollection<JadwalOption> filteredTahunAkademikList;
    [ObservableProperty] private ObservableCollection<JadwalOption> filteredDosenList;
    [ObservableProperty] private ObservableCollection<JadwalOption> filteredHariList;
    [ObservableProperty] private ObservableCollection<JadwalOption> filteredWaktuList;
    [ObservableProperty] private ObservableCollection<JadwalOption> filteredRuanganList;
    [ObservableProperty] private ObservableCollection<JadwalOption> filteredKelasList;


    [ObservableProperty] private string mataKuliahSearchText;
    partial void OnMataKuliahSearchTextChanged(string value) => FilterList(value, _allMataKuliah, list => FilteredMataKuliahList = list);

    [ObservableProperty] private string tahunAkademikSearchText;
    partial void OnTahunAkademikSearchTextChanged(string value) => FilterList(value, _allTahunAkademik, list => FilteredTahunAkademikList = list);

    [ObservableProperty] private string dosenSearchText;
    partial void OnDosenSearchTextChanged(string value) => FilterList(value, _allDosen, list => FilteredDosenList = list);

    [ObservableProperty] private string hariSearchText;
    partial void OnHariSearchTextChanged(string value) => FilterList(value, _allHari, list => FilteredHariList = list);

    [ObservableProperty] private string waktuSearchText;
    partial void OnWaktuSearchTextChanged(string value) => FilterList(value, _allWaktu, list => FilteredWaktuList = list);

    [ObservableProperty] private string ruanganSearchText;
    partial void OnRuanganSearchTextChanged(string value) => FilterList(value, _allRuangan, list => FilteredRuanganList = list);

    [ObservableProperty] private string kelasSearchText;
    partial void OnKelasSearchTextChanged(string value) => FilterList(value, _allKelas, list => FilteredKelasList = list);

    private void FilterList(string value, List<JadwalOption> source, Action<ObservableCollection<JadwalOption>> setter)
    {
        if (source == null) return;

        var filtered = string.IsNullOrWhiteSpace(value)
            ? source
            : source.Where(x => x.Label.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList();

        setter(new ObservableCollection<JadwalOption>(filtered));
    }


    [ObservableProperty, NotifyPropertyChangedFor(nameof(MataKuliahLabel))] private JadwalOption selectedMataKuliah;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(TahunAkademikLabel))] private JadwalOption selectedTahunAkademik;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(DosenLabel))] private JadwalOption selectedDosen;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(HariLabel))] private JadwalOption selectedHari;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(WaktuLabel))] private JadwalOption selectedWaktu;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(RuanganLabel))] private JadwalOption selectedRuangan;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KelasLabel))] private JadwalOption selectedKelas;


    public string MataKuliahLabel => SelectedMataKuliah?.Label ?? "Pilih Mata Kuliah";
    public string TahunAkademikLabel => SelectedTahunAkademik?.Label ?? "Pilih Tahun Akademik";
    public string DosenLabel => SelectedDosen?.Label ?? "Pilih Dosen";
    public string HariLabel => SelectedHari?.Label ?? "Pilih Hari";
    public string WaktuLabel => SelectedWaktu?.Label ?? "Pilih Waktu";
    public string RuanganLabel => SelectedRuangan?.Label ?? "Pilih Ruangan";
    public string KelasLabel => SelectedKelas?.Label ?? "Pilih Kelas";

 
    [ObservableProperty] private bool isMataKuliahDropdownVisible;
    [ObservableProperty] private bool isTahunAkademikDropdownVisible;
    [ObservableProperty] private bool isDosenDropdownVisible;
    [ObservableProperty] private bool isHariDropdownVisible;
    [ObservableProperty] private bool isWaktuDropdownVisible;
    [ObservableProperty] private bool isRuanganDropdownVisible;
    [ObservableProperty] private bool isKelasDropdownVisible;

    private void CloseAllDropdowns()
    {
        IsMataKuliahDropdownVisible = false;
        IsTahunAkademikDropdownVisible = false;
        IsDosenDropdownVisible = false;
        IsHariDropdownVisible = false;
        IsWaktuDropdownVisible = false;
        IsRuanganDropdownVisible = false;
        IsKelasDropdownVisible = false;
    }


    [RelayCommand] private void ToggleMataKuliahDropdown() { CloseAllDropdowns(); IsMataKuliahDropdownVisible = !IsMataKuliahDropdownVisible; }
    [RelayCommand] private void ToggleTahunAkademikDropdown() { CloseAllDropdowns(); IsTahunAkademikDropdownVisible = !IsTahunAkademikDropdownVisible; }
    [RelayCommand] private void ToggleDosenDropdown() { CloseAllDropdowns(); IsDosenDropdownVisible = !IsDosenDropdownVisible; }
    [RelayCommand] private void ToggleHariDropdown() { CloseAllDropdowns(); IsHariDropdownVisible = !IsHariDropdownVisible; }
    [RelayCommand] private void ToggleWaktuDropdown() { CloseAllDropdowns(); IsWaktuDropdownVisible = !IsWaktuDropdownVisible; }
    [RelayCommand] private void ToggleRuanganDropdown() { CloseAllDropdowns(); IsRuanganDropdownVisible = !IsRuanganDropdownVisible; }
    [RelayCommand] private void ToggleKelasDropdown() { CloseAllDropdowns(); IsKelasDropdownVisible = !IsKelasDropdownVisible; }


    [RelayCommand]
    public async Task LoadDropdownsAsync()
    {
        try
        {
            _allMataKuliah = await _service.GetMataKuliahAsync();
            FilteredMataKuliahList = new ObservableCollection<JadwalOption>(_allMataKuliah);

            _allTahunAkademik = await _service.GetTahunAkademikAsync();
            FilteredTahunAkademikList = new ObservableCollection<JadwalOption>(_allTahunAkademik);

            _allDosen = await _service.GetDosenAsync();
            FilteredDosenList = new ObservableCollection<JadwalOption>(_allDosen);

            _allHari = await _service.GetHariAsync();
            FilteredHariList = new ObservableCollection<JadwalOption>(_allHari);

            _allWaktu = await _service.GetWaktuAsync();
            FilteredWaktuList = new ObservableCollection<JadwalOption>(_allWaktu);

            _allRuangan = await _service.GetRuanganAsync();
            FilteredRuanganList = new ObservableCollection<JadwalOption>(_allRuangan);

            _allKelas = await _service.GetKelasAsync();
            FilteredKelasList = new ObservableCollection<JadwalOption>(_allKelas);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Gagal memuat data: {ex.Message}", "OK");
        }
    }


    [RelayCommand]
    public async Task SimpanJadwalAsync()
    {
        if (SelectedMataKuliah == null ||
            SelectedTahunAkademik == null ||
            SelectedDosen == null ||
            SelectedHari == null ||
            SelectedWaktu == null ||
            SelectedRuangan == null ||
            SelectedKelas == null)
        {
            await Shell.Current.DisplayAlert("Gagal", "Semua field harus dipilih sebelum menyimpan.", "OK");
            return;
        }

        var jadwal = new JadwalInputModel
        {
            KodeMK = SelectedMataKuliah.Id,
            TahunAkademik = SelectedTahunAkademik.Id,
            Dosen = SelectedDosen.Id,
            Hari = SelectedHari.Id,
            Waktu = SelectedWaktu.Id,
            Ruangan = SelectedRuangan.Id,
            Kelas = SelectedKelas.Id
        };

        try
        {
            var sukses = await _service.SimpanJadwalAsync(jadwal);
            if (sukses)
            {
                await Shell.Current.DisplayAlert("Sukses", "Jadwal berhasil disimpan", "OK");
                ResetForm();
            }
            else
            {
                await Shell.Current.DisplayAlert("Gagal", "Server menolak data jadwal", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Gagal menyimpan jadwal: {ex.Message}", "OK");
        }
    }

    private void ResetForm()
    {
        SelectedMataKuliah = null;
        SelectedTahunAkademik = null;
        SelectedDosen = null;
        SelectedHari = null;
        SelectedWaktu = null;
        SelectedRuangan = null;
        SelectedKelas = null;
    }
}
