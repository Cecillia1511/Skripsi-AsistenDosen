using LoginApp.Maui.Models;

namespace LoginApp.Maui.Services;

public interface IJadwalService
{
    Task<List<JadwalOption>> GetMataKuliahAsync();
    Task<List<JadwalOption>> GetTahunAkademikAsync();
    Task<List<JadwalOption>> GetDosenAsync();
    Task<List<JadwalOption>> GetHariAsync();
    Task<List<JadwalOption>> GetWaktuAsync();
    Task<List<JadwalOption>> GetRuanganAsync();
    Task<List<JadwalOption>> GetKelasAsync();
    Task<List<JadwalTampil>> GetJadwalByTahunAsync(int tahunId);
    Task<bool> SimpanJadwalAsync(JadwalInputModel jadwal);
}

