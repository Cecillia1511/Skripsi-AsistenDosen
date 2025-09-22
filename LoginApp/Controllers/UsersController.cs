using Dapper;
using LoginApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using LoginApp.Api.Data;

namespace LoginApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public UsersController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            using var con = GetConnection();
            var users = await con.QueryAsync<User>("SELECT * FROM [User]");
            return Ok(users);
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var allowedRoleIds = new[] { 1, 2, 3, 4, 5 };

            if (string.IsNullOrWhiteSpace(user.Username) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(user.Nama) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                user.Role_ID == 0)
            {
                return BadRequest(new Response
                {
                    StatusCode = 400,
                    StatusMessage = "Nama, Username, Password, Email, dan Role_ID wajib diisi"
                });
            }

            if (!allowedRoleIds.Contains(user.Role_ID))
            {
                return BadRequest(new Response
                {
                    StatusCode = 400,
                    StatusMessage = "Role_ID tidak valid. Gunakan salah satu dari: 1 (Koordinator), 2 (AsistenDosen), 3 (Akademik), 4 (Kaprodi), 5 (Mahasiswa)"
                });
            }

            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            con.Open();

            // 🔍 Cek apakah username sudah ada
            using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM [User] WHERE LOWER(Username) = @Username", con);
            checkCmd.Parameters.AddWithValue("@Username", user.Username.ToLower());
            int count = (int)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                return BadRequest(new Response
                {
                    StatusCode = 400,
                    StatusMessage = "Username sudah digunakan"
                });
            }

            // 🔐 Insert user baru
            using var cmd = new SqlCommand(@"
                INSERT INTO [User] (Nama, Username, Password, Email, Role_ID)
                VALUES (@Nama, @Username, @Password, @Email, @Role_ID)", con);

            cmd.Parameters.AddWithValue("@Nama", user.Nama);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(user.Password));
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Role_ID", user.Role_ID);

            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
            {
                return Ok(new Response
                {
                    StatusCode = 200,
                    StatusMessage = "Registrasi berhasil"
                });
            }

            return BadRequest(new Response
            {
                StatusCode = 400,
                StatusMessage = "Registrasi gagal"
            });
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserRequest user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new Response
                {
                    StatusCode = 400,
                    StatusMessage = "Username and Password are required"
                });
            }

            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand("SELECT * FROM [User] WHERE LOWER(Username) = @Username", con);
            cmd.Parameters.AddWithValue("@Username", user.Username.ToLower());

            con.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return Unauthorized(new Response
                {
                    StatusCode = 401,
                    StatusMessage = "Invalid username or password"
                });
            }

            var storedHashedPassword = reader["Password"].ToString();
            bool isValid = BCrypt.Net.BCrypt.Verify(user.Password, storedHashedPassword);

            if (!isValid)
            {
                return Unauthorized(new Response
                {
                    StatusCode = 401,
                    StatusMessage = "Invalid username or password"
                });
            }

            int roleId = Convert.ToInt32(reader["Role_ID"]);
            string roleName = roleId switch
            {
                1 => "Koordinator",
                2 => "Asisten Dosen",
                3 => "Akademik",
                4 => "Kaprodi",
                5 => "Mahasiswa",
                _ => "Tidak dikenal"
            };

            var foundUser = new
            {
                User_ID = Convert.ToInt32(reader["User_ID"]),
                Nama = reader["Nama"].ToString(),
                Username = reader["Username"].ToString(),
                Email = reader["Email"].ToString(),
                Role = roleName
            };

            return Ok(new
            {
                StatusCode = 200,
                StatusMessage = "Login successful",
                Data = foundUser
            });
        }


        [HttpGet("AsistenDosen")]
        public IActionResult GetAsistenDosen()
        {
            var result = new List<object>();

            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand("SELECT User_ID, Nama, Username, Email FROM [User] WHERE Role_ID = @Role_ID", con);
            cmd.Parameters.AddWithValue("@Role_ID", 2);

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var user = new
                {
                    User_ID = Convert.ToInt32(reader["User_ID"]),
                    Nama = reader["Nama"].ToString(),
                    Username = reader["Username"].ToString(),
                    Email = reader["Email"].ToString(),
                    Role = "Asisten Dosen"
                };

                result.Add(user);
            }

            con.Close();
            return Ok(new
            {
                StatusCode = 200,
                StatusMessage = "Data ditemukan",
                Data = result
            });
        }




        //[HttpGet("persemester")]
        //public IActionResult GetAsistenPerSemester([FromQuery] string semester)
        //{
        //    var rawList = new List<AsistenDosenViewModel>();

        //    using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
        //    using var cmd = new SqlCommand(@"
        //        SELECT 
        //            a.AsistenDosenId AS AsistenId,
        //            u.Name,
        //            u.Username,
        //            a.Semester,
        //            mk.KodeMK,
        //            mk.NamaMK AS MataKuliah,
        //            k.NamaKelas AS Kelas,
        //            a.StatusVerifikasi
        //        FROM AsistenDosen a
        //        JOIN [User] u ON a.UserId = u.UserId
        //        JOIN MataKuliah mk ON a.MataKuliahId = mk.MataKuliahId
        //        JOIN Kelas k ON a.KelasId = k.KelasId
        //        WHERE a.Semester = @Semester", con);

        //    cmd.Parameters.AddWithValue("@Semester", semester);

        //    con.Open();
        //    using var reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        rawList.Add(new AsistenDosenViewModel
        //        {
        //            AsistenId = Convert.ToInt32(reader["AsistenId"]),
        //            Name = reader["Name"].ToString(),
        //            Username = reader["Username"].ToString(),
        //            Semester = reader["Semester"].ToString(),
        //            StatusVerifikasi = Convert.ToBoolean(reader["StatusVerifikasi"]),

        //            // langsung simpan ke list biar rapi
        //            MataKuliahList = new List<MataKuliahModel>
        //    {
        //        new MataKuliahModel
        //        {
        //            KodeMK = reader["KodeMK"].ToString(),
        //            Nama = reader["MataKuliah"].ToString(),
        //            Kelas = reader["Kelas"].ToString()
        //        }
        //    }
        //        });
        //    }

        //    // Grouping per asisten (unik)
        //    var grouped = rawList
        //        .GroupBy(x => new { x.Username, x.Name, x.Semester, x.StatusVerifikasi })
        //        .Select(g => new AsistenDosenViewModel
        //        {
        //            AsistenId = g.First().AsistenId,
        //            Name = g.Key.Name,
        //            Username = g.Key.Username,
        //            Semester = g.Key.Semester,
        //            StatusVerifikasi = g.Key.StatusVerifikasi,
        //            MataKuliahList = g
        //                .SelectMany(x => x.MataKuliahList)
        //                .GroupBy(mk => new { mk.KodeMK, mk.Nama, mk.Kelas })
        //                .Select(mk => new MataKuliahModel
        //                {
        //                    KodeMK = mk.Key.KodeMK,
        //                    Nama = mk.Key.Nama,
        //                    Kelas = mk.Key.Kelas
        //                })
        //                .OrderBy(mk => mk.KodeMK)   // ✅ Urutkan by KodeMK
        //                .ThenBy(mk => mk.Kelas)     // ✅ Kalau sama, urutkan by Kelas
        //                .ToList()
        //        })
        //        .OrderBy(a => a.Username)  // ✅ Asisten urut by NIM
        //        .ToList();


        //    return Ok(grouped);
        //}


        //[HttpGet("nama")]
        //public IActionResult GetNamaByUsername([FromQuery] string username)
        //{
        //    using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
        //    using var cmd = new SqlCommand("SELECT Name FROM [User] WHERE LOWER(Username) = @Username AND IsActive = 1", con);
        //    cmd.Parameters.AddWithValue("@Username", username.ToLower());

        //    con.Open();
        //    var result = cmd.ExecuteScalar();

        //    if (result != null)
        //    {
        //        var name = result.ToString();
        //        return Ok(new { Name = name });
        //    }

        //    return NotFound(new { StatusCode = 404, StatusMessage = "Nama tidak ditemukan" });
        //}




    }
}
