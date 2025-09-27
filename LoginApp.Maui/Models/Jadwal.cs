namespace LoginApp.Maui.Models;

public class JadwalOption
{
    public string Id { get; set; }
    public string Label { get; set; }
}

// Model untuk UI (sama seperti sebelumnya, tapi tambah optional)
public class JadwalTampil
{
    public string KodeMK { get; set; } = "";
    public string NamaMK { get; set; } = "";
    public int SKS { get; set; } = 0;
    public string Kelas { get; set; } = "";
    public string NIP { get; set; } = "";
    public string NamaDosen { get; set; } = "";
    public string Hari { get; set; } = "";
    public string WaktuMulai { get; set; } = "";
    public string WaktuSelesai { get; set; } = "";
    public string Ruangan { get; set; } = "";
}

public class JadwalResponse
{
    public int jadwal_ID { get; set; }
    public string kodeMK { get; set; }
    public string mataKuliah { get; set; }
    public string tahunAkademik { get; set; }
    public string sks { get; set; }
    public string nip { get; set; }
    public string dosen { get; set; }
    public string hari { get; set; }
    public string jamMulai { get; set; }
    public string jamSelesai { get; set; }
    public string ruangan { get; set; }
    public string kelas { get; set; }
}

public class JadwalTampilDto
{
    public string KodeMK { get; set; }
    public string NamaMK { get; set; }
    public int SKS { get; set; }
    public string NIP { get; set; }
    public string NamaDosen { get; set; }
    public string Hari { get; set; }
    public string WaktuMulai { get; set; }
    public string WaktuSelesai { get; set; }
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
