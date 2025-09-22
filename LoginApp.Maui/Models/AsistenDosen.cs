using System.Text.Json.Serialization;

public class MataKuliahModel
{
    public string KodeMK { get; set; }
    public string Nama { get; set; } = string.Empty;
    public string Kelas { get; set; } = string.Empty;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(Kelas) ? Nama : $"{Nama} ({Kelas})";
}

public class AsistenDosenViewModel
{
    public int AsistenId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public bool StatusVerifikasi { get; set; }  

    // Langsung list, bukan string yang diparse
    public List<MataKuliahModel> MataKuliahList { get; set; } = new();

    // Tidak perlu [JsonIgnore] lagi
    // MataKuliah + Kelas sebagai string dihapus biar ga rancu

    public string Prodi =>
        string.IsNullOrWhiteSpace(Username) ? "Tidak Diketahui" :
        Username.StartsWith("825") ? "Sistem Informasi" :
        Username.StartsWith("535") ? "Teknik Informatika" :
        "Prodi Lain";
}
