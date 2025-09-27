using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApp.Maui.Models;
using LoginApp.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LoginApp.Maui.ViewModels;  // Sesuaikan namespace kalau beda (misalnya ViewsAkademik)

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

        // ObservableCollection otomatis refresh di UI
        TahunAkademikList = new ObservableCollection<JadwalOption>();
        JadwalList = new ObservableCollection<JadwalTampil>();
    }

    // 🔹 PROPERTIES
    [ObservableProperty]
    private string namaUser;

    [ObservableProperty]
    private ObservableCollection<JadwalOption> tahunAkademikList;

    [ObservableProperty]
    private JadwalOption selectedTahunAkademik;

    [ObservableProperty]
    private bool isTahunDropdownVisible;

    [ObservableProperty]
    private ObservableCollection<JadwalTampil> jadwalList;

    // 🔹 COMMANDS
    [RelayCommand]
    private void ToggleTahunDropdown() =>
        IsTahunDropdownVisible = !IsTahunDropdownVisible;

    // 🔹 INIT
    public async Task InitializeAsync()
    {
        try
        {
            Debug.WriteLine("[VIEWMODEL] Initializing HomePageAkademik...");
            await LoadTahunAkademikAsync();

            if (TahunAkademikList.Count > 0)
            {
                SelectedTahunAkademik = TahunAkademikList[0];
                await LoadJadwalAsync();  // Load awal untuk selected pertama
                Debug.WriteLine("[VIEWMODEL] Initialization complete.");
            }
            else
            {
                Debug.WriteLine("[VIEWMODEL] No tahun akademik available.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[VIEWMODEL] InitializeAsync error: {ex.Message}");
            // Optional: Tampil alert ke user (uncomment kalau mau)
            // await Application.Current.MainPage.DisplayAlert("Error", "Gagal load data awal: " + ex.Message, "OK");
        }
    }

    // 🔹 LOAD TAHUN AKADEMIK (FIXED: Tambah try-catch & log)
    private async Task LoadTahunAkademikAsync()
    {
        try
        {
            Debug.WriteLine("[VIEWMODEL] Loading tahun akademik...");
            var result = await _tahunService.GetDropdownAsync();  // Asumsi method ini ada di ITahunAkademikService

            TahunAkademikList.Clear();
            if (result != null)
            {
                foreach (var item in result)
                    TahunAkademikList.Add(item);
                Debug.WriteLine($"[VIEWMODEL] Loaded {TahunAkademikList.Count} tahun akademik items.");
            }
            else
            {
                Debug.WriteLine("[VIEWMODEL] GetDropdownAsync returned null.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[VIEWMODEL] LoadTahunAkademikAsync error: {ex.Message}");
            // Optional: Alert
            // await Application.Current.MainPage.DisplayAlert("Error", "Gagal load tahun akademik: " + ex.Message, "OK");
        }
    }

    // 🔹 LOAD JADWAL BERDASARKAN TAHUN (FIXED: Try-catch, log improved, Debug.WriteLine dengan NamaDosen & quotes)
    public async Task LoadJadwalAsync()
    {
        if (SelectedTahunAkademik == null)
        {
            Debug.WriteLine("[VIEWMODEL] SelectedTahunAkademik is null, skipping LoadJadwalAsync.");
            return;
        }

        try
        {
            Debug.WriteLine($"[VIEWMODEL] Loading jadwal for tahun ID: {SelectedTahunAkademik.Id} (Label: {SelectedTahunAkademik.Label})");

            var rawData = await _jadwalService.GetJadwalByTahunAsync(SelectedTahunAkademik.Id);

            Debug.WriteLine($"[VIEWMODEL] Received {(rawData?.Count ?? 0)} items from service.");

            JadwalList.Clear();
            if (rawData != null && rawData.Count > 0)
            {
                foreach (var item in rawData)
                {
                    Debug.WriteLine($"KodeMK: '{item.KodeMK}', NamaMK: '{item.NamaMK}', SKS: {item.SKS}, NIP: '{item.NIP}', NamaDosen: '{item.NamaDosen}', Hari: '{item.Hari}', Mulai: '{item.WaktuMulai}', Selesai: '{item.WaktuSelesai}'");

                    JadwalList.Add(new JadwalTampil  // Keep seperti asli
                    {
                        KodeMK = item.KodeMK,
                        NamaMK = item.NamaMK,
                        SKS = item.SKS,
                        Kelas = item.Kelas ?? "",
                        NIP = item.NIP,
                        NamaDosen = item.NamaDosen,
                        Hari = item.Hari,
                        WaktuMulai = item.WaktuMulai ?? "",
                        WaktuSelesai = item.WaktuSelesai ?? "",
                        Ruangan = item.Ruangan ?? ""
                    });
                }
                Debug.WriteLine($"[VIEWMODEL] Loaded {JadwalList.Count} jadwal items successfully.");
            }
            else
            {
                Debug.WriteLine("[VIEWMODEL] rawData is null or empty (0 items). Check service logs for JSON issues.");
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[VIEWMODEL] LoadJadwalAsync error: {ex.Message}");
            JadwalList.Clear();  // Clear list kalau error, biar UI gak stuck
            // Optional: Alert
            // await Application.Current.MainPage.DisplayAlert("Error", "Gagal load jadwal: " + ex.Message, "OK");
        }
    }
}
