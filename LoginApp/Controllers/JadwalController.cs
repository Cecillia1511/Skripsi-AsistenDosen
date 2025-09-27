using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace LoginApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JadwalController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public JadwalController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class JadwalRequest
        {
            public string KodeMK { get; set; }
            public string TahunAkademik { get; set; }
            public string Dosen { get; set; }
            public string Hari { get; set; }
            public string Waktu { get; set; }
            public string Ruangan { get; set; }
            public string Kelas { get; set; }
        }


        [HttpPost("Input")]
        public IActionResult InputJadwal([FromBody] JadwalRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    StatusMessage = "Request tidak valid"
                });
            }

            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));

            using var checkCmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM Jadwal
            WHERE Kode_MK = @Kode_MK
              AND TahunAkademik_ID = @TahunAkademik_ID
              AND NIP = @NIP
              AND Hari_ID = @Hari_ID
              AND Waktu_ID = @Waktu_ID
              AND Ruangan_ID = @Ruangan_ID
              AND Kelas_ID = @Kelas_ID", con);

            checkCmd.Parameters.AddWithValue("@Kode_MK", request.KodeMK);
            checkCmd.Parameters.AddWithValue("@TahunAkademik_ID", request.TahunAkademik);
            checkCmd.Parameters.AddWithValue("@NIP", request.Dosen);
            checkCmd.Parameters.AddWithValue("@Hari_ID", request.Hari);
            checkCmd.Parameters.AddWithValue("@Waktu_ID", request.Waktu);
            checkCmd.Parameters.AddWithValue("@Ruangan_ID", request.Ruangan);
            checkCmd.Parameters.AddWithValue("@Kelas_ID", request.Kelas);

            try
            {
                con.Open();

                var exists = (int)checkCmd.ExecuteScalar() > 0;
                if (exists)
                {
                    return Conflict(new
                    {
                        StatusCode = 409,
                        StatusMessage = "Jadwal sudah ada, tidak boleh duplikat."
                    });
                }

                using var cmd = new SqlCommand(@"
                INSERT INTO Jadwal (Kode_MK, TahunAkademik_ID, NIP, Hari_ID, Waktu_ID, Ruangan_ID, Kelas_ID)
                VALUES (@Kode_MK, @TahunAkademik_ID, @NIP, @Hari_ID, @Waktu_ID, @Ruangan_ID, @Kelas_ID)", con);

                cmd.Parameters.AddWithValue("@Kode_MK", request.KodeMK);
                cmd.Parameters.AddWithValue("@TahunAkademik_ID", request.TahunAkademik);
                cmd.Parameters.AddWithValue("@NIP", request.Dosen);
                cmd.Parameters.AddWithValue("@Hari_ID", request.Hari);
                cmd.Parameters.AddWithValue("@Waktu_ID", request.Waktu);
                cmd.Parameters.AddWithValue("@Ruangan_ID", request.Ruangan);
                cmd.Parameters.AddWithValue("@Kelas_ID", request.Kelas);

                int rows = cmd.ExecuteNonQuery();

                return Ok(new
                {
                    StatusCode = 200,
                    StatusMessage = "Jadwal berhasil disimpan",
                    RowsAffected = rows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    StatusMessage = "Terjadi kesalahan saat menyimpan jadwal",
                    Error = ex.Message
                });
            }
        }




        [HttpGet("All")]
        public IActionResult GetAllJadwal()
        {
            var result = new List<object>();

            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand(@"
            SELECT 
                j.Jadwal_ID,
                mk.NamaMK,
                mk.SKS,
                ta.Tahun,
                d.NamaDosen,
                h.NamaHari,
                w.WaktuMulai,
                w.WaktuSelesai,
                r.NamaRuangan,
                k.NamaKelas
            FROM Jadwal j
            JOIN MataKuliahPraktikum mk ON j.Kode_MK = mk.Kode_MK
            JOIN TahunAkademik ta ON j.TahunAkademik_ID = ta.TahunAkademik_ID
            JOIN Dosen d ON j.NIP = d.NIP
            JOIN Hari h ON j.Hari_ID = h.Hari_ID
            JOIN Waktu w ON j.Waktu_ID = w.Waktu_ID
            JOIN Ruangan r ON j.Ruangan_ID = r.Ruangan_ID
            JOIN Kelas k ON j.Kelas_ID = k.Kelas_ID
            ", con);

            try
            {
                con.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var jadwal = new
                    {
                        Jadwal_ID = Convert.ToInt32(reader["Jadwal_ID"]),
                        MataKuliah = reader["NamaMK"].ToString(),
                        TahunAkademik = reader["Tahun"].ToString(),
                        Dosen = reader["NamaDosen"].ToString(),
                        Hari = reader["NamaHari"].ToString(),
                        JamMulai = reader["WaktuMulai"].ToString(),
                        JamSelesai = reader["WaktuSelesai"].ToString(),
                        Ruangan = reader["NamaRuangan"].ToString(),
                        Kelas = reader["NamaKelas"].ToString()
                    };

                    result.Add(jadwal);
                }

                return Ok(new
                {
                    StatusCode = 200,
                    StatusMessage = "Data jadwal ditemukan",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    StatusMessage = "Terjadi kesalahan saat mengambil data jadwal",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("tahun/{tahunId}")]
        public IActionResult GetByTahun(string tahunId)
        {
            var result = new List<object>();
            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand(@"
                SELECT j.Jadwal_ID, ta.Tahun, j.Kode_MK, mk.NamaMK, k.NamaKelas, mk.SKS, j.NIP, d.NamaDosen, h.NamaHari,
                       w.WaktuMulai, w.WaktuSelesai, r.NamaRuangan
                FROM Jadwal j
                JOIN MataKuliahPraktikum mk ON j.Kode_MK = mk.Kode_MK
                JOIN TahunAkademik ta ON j.TahunAkademik_ID = ta.TahunAkademik_ID
                JOIN Dosen d ON j.NIP = d.NIP
                JOIN Hari h ON j.Hari_ID = h.Hari_ID
                JOIN Waktu w ON j.Waktu_ID = w.Waktu_ID
                JOIN Ruangan r ON j.Ruangan_ID = r.Ruangan_ID
                JOIN Kelas k ON j.Kelas_ID = k.Kelas_ID
                WHERE j.TahunAkademik_ID = @tahunId
                ORDER BY j.Kode_MK, k.NamaKelas", 
                con);
            cmd.Parameters.AddWithValue("@tahunId", tahunId);

            try
            {
                con.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        jadwal_ID = Convert.ToInt32(reader["Jadwal_ID"]),  // Lowercase untuk camelCase JSON
                        kodeMK = reader["Kode_MK"]?.ToString() ?? "",     // TAMBAH: KodeMK (string, handle null)
                        mataKuliah = reader["NamaMK"]?.ToString() ?? "",  // Sudah ada, tambah null-safe
                        tahunAkademik = reader["Tahun"]?.ToString() ?? "", // Sudah ada
                        sks = reader["SKS"]?.ToString()?.Trim() ?? "0",   // Sudah ada, tambah Trim() biar gak spaces
                        nip = reader["NIP"]?.ToString() ?? "",            // TAMBAH: NIP (string, handle null)
                        dosen = reader["NamaDosen"]?.ToString() ?? "",    // Sudah ada
                        hari = reader["NamaHari"]?.ToString() ?? "",      // Sudah ada
                        jamMulai = reader["WaktuMulai"]?.ToString() ?? "", // Sudah ada
                        jamSelesai = reader["WaktuSelesai"]?.ToString() ?? "", // Sudah ada
                        ruangan = reader["NamaRuangan"]?.ToString() ?? "", // Sudah ada
                        kelas = reader["NamaKelas"]?.ToString() ?? ""     // Sudah ada
                    });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusMessage = "Error", Error = ex.Message });
            }
        }



    }
}
