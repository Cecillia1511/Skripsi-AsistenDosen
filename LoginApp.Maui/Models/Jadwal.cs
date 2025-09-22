namespace LoginApp.Maui.Models;

public class JadwalOption
{
    public string Id { get; set; }
    public string Label { get; set; }
}

public class JadwalTampil
{
    public int JadwalId { get; set; }
    public string MataKuliah { get; set; }
    public string TahunAkademik { get; set; }
    public string Dosen { get; set; }
    public string Hari { get; set; }
    public string Waktu { get; set; }
    public string Ruangan { get; set; }
    public string Kelas { get; set; }
}

public class JadwalInputModel
{
    public string KodeMK { get; set; }
    public string MataKuliah { get; set; }
    public string Dosen { get; set; }
    public string Hari { get; set; }
    public string Waktu { get; set; }
    public string Ruangan { get; set; }
    public string Kelas { get; set; }
    public string Prodi { get; set; }
    public string TahunAkademik { get; set; }
}

public class JadwalPerkuliahan
{
    public string KodeMK { get; set; }
    public string MataKuliah { get; set; }
    public string Dosen { get; set; }
    public string Hari { get; set; }
    public string Waktu { get; set; }
    public string Ruangan { get; set; }
    public string Kelas { get; set; }
    public string Prodi { get; set; }
}

