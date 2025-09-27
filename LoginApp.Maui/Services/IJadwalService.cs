using LoginApp.Maui.Models;

namespace LoginApp.Maui.Services;

public interface IJadwalService
{
    Task<List<JadwalOption>> GetMataKuliahAsync();
    Task<List<JadwalTampil>> GetJadwalByTahunAsync(string tahunId);
    Task<List<JadwalOption>> GetDosenAsync();
    Task<List<JadwalOption>> GetHariAsync();
    Task<List<JadwalOption>> GetWaktuAsync();
    Task<List<JadwalOption>> GetRuanganAsync();
    Task<List<JadwalOption>> GetKelasAsync();
    Task<(bool Success, string Message)> SimpanJadwalAsync(JadwalInputModel jadwal);
}


