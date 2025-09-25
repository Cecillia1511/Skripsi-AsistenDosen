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
            public string Kode_MK { get; set; }              
            public string TahunAkademik_ID { get; set; }     
            public string NIP { get; set; }                  
            public string Hari_ID { get; set; }              
            public string Waktu_ID { get; set; }            
            public string Ruangan_ID { get; set; }          
            public string Kelas_ID { get; set; }            
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
            using var cmd = new SqlCommand(@"
            INSERT INTO Jadwal (Kode_MK, TahunAkademik_ID, NIP, Hari_ID, Waktu_ID, Ruangan_ID, Kelas_ID)
            VALUES (@Kode_MK, @TahunAkademik_ID, @NIP, @Hari_ID, @Waktu_ID, @Ruangan_ID, @Kelas_ID)", con);

            cmd.Parameters.AddWithValue("@Kode_MK", request.Kode_MK);
            cmd.Parameters.AddWithValue("@TahunAkademik_ID", request.TahunAkademik_ID);
            cmd.Parameters.AddWithValue("@NIP", request.NIP);
            cmd.Parameters.AddWithValue("@Hari_ID", request.Hari_ID);
            cmd.Parameters.AddWithValue("@Waktu_ID", request.Waktu_ID);
            cmd.Parameters.AddWithValue("@Ruangan_ID", request.Ruangan_ID);
            cmd.Parameters.AddWithValue("@Kelas_ID", request.Kelas_ID);


            try
            {
                con.Open();
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
        public IActionResult GetByTahun(int tahunId)
        {
            var result = new List<object>();
            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand(@"
        SELECT j.Jadwal_ID, mk.NamaMK, ta.Tahun, d.NamaDosen, h.NamaHari,
               w.WaktuMulai, w.WaktuSelesai, r.NamaRuangan, k.NamaKelas
        FROM Jadwal j
        JOIN MataKuliahPraktikum mk ON j.Kode_MK = mk.Kode_MK
        JOIN TahunAkademik ta ON j.TahunAkademik_ID = ta.TahunAkademik_ID
        JOIN Dosen d ON j.NIP = d.NIP
        JOIN Hari h ON j.Hari_ID = h.Hari_ID
        JOIN Waktu w ON j.Waktu_ID = w.Waktu_ID
        JOIN Ruangan r ON j.Ruangan_ID = r.Ruangan_ID
        JOIN Kelas k ON j.Kelas_ID = k.Kelas_ID
        WHERE j.TahunAkademik_ID = @tahunId", con);
            cmd.Parameters.AddWithValue("@tahunId", tahunId);

            try
            {
                con.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
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
