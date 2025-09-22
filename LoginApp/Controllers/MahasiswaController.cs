using Google.Apis.Auth;
using LoginApp.Api.Helpers;
using LoginApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LoginApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MahasiswaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtHelper _jwtHelper;

        public MahasiswaController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtHelper = new JwtHelper(configuration);
        }

        // 🔐 Login Manual Mahasiswa

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult LoginMahasiswa([FromBody] Mahasiswa mahasiswa)
        {
            if (string.IsNullOrWhiteSpace(mahasiswa.Gmail) || string.IsNullOrWhiteSpace(mahasiswa.Password))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gmail dan Password wajib diisi"
                });
            }

            //Microsoft.Extensions.Primitives.StringValues TokenFromClient;
            // Request.Headers.TryGetValue("X-Token", out TokenFromClient);

            //if (TokenFromClient!="yg ada di database") return Unauthorized(new
            //{
            //    StatusCode = 401,
            //    Message = "Not authorized"
            //});

            // Validasi domain UNTAR
            var allowedDomains = new[] { "@stu.untar.ac.id", "@untar.ac.id" };
            var email = mahasiswa.Gmail.Trim().ToLower();

            if (!allowedDomains.Any(domain => email.EndsWith(domain)))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gmail bukan domain UNTAR"
                });
            }

            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand("SELECT * FROM Mahasiswa WHERE LOWER(Gmail) = @Gmail", con);
            cmd.Parameters.AddWithValue("@Gmail", email);

            con.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return Unauthorized(new
                {
                    StatusCode = 401,
                    Message = "Gmail tidak ditemukan"
                });
            }

            var storedHash = reader["Password"].ToString();
            bool isValid = BCrypt.Net.BCrypt.Verify(mahasiswa.Password, storedHash);

            if (!isValid)
            {
                return Unauthorized(new
                {
                    StatusCode = 401,
                    Message = "Password salah"
                });
            }

            // Generate token
            var token = _jwtHelper.GenerateToken(email);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Login berhasil",
                Token = token,
                Gmail = email,
                Role = "mahasiswa"
            });
        }


        // 🔐 Register Mahasiswa
        [HttpPost("register")]
        public IActionResult RegisterMahasiswa([FromBody] Mahasiswa mahasiswa)
        {
            if (string.IsNullOrWhiteSpace(mahasiswa.Gmail) || string.IsNullOrWhiteSpace(mahasiswa.Password))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gmail dan Password wajib diisi"
                });
            }

            var allowedDomains = new[] { "@stu.untar.ac.id", "@untar.ac.id" };
            var email = mahasiswa.Gmail.Trim().ToLower();

            if (!allowedDomains.Any(domain => email.EndsWith(domain)))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Gmail bukan domain UNTAR"
                });
            }

            using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
            con.Open();

            // Cek apakah Gmail sudah terdaftar
            using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Mahasiswa WHERE LOWER(Gmail) = @Gmail", con);
            checkCmd.Parameters.AddWithValue("@Gmail", email);
            int existingCount = (int)checkCmd.ExecuteScalar();

            if (existingCount > 0)
            {
                return Conflict(new
                {
                    StatusCode = 409,
                    Message = "Gmail sudah terdaftar"
                });
            }

            // Hash password dan simpan
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(mahasiswa.Password);

            using var insertCmd = new SqlCommand("INSERT INTO Mahasiswa (Gmail, Password) VALUES (@Gmail, @Password)", con);
            insertCmd.Parameters.AddWithValue("@Gmail", email);
            insertCmd.Parameters.AddWithValue("@Password", hashedPassword);

            int result = insertCmd.ExecuteNonQuery();

            if (result > 0)
            {
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Registrasi berhasil",
                    Gmail = email
                });
            }

            return StatusCode(500, new
            {
                StatusCode = 500,
                Message = "Terjadi kesalahan saat registrasi"
            });
        }
    }

    //// 🔐 Login Mahasiswa via Google OAuth
    //[HttpPost("login-google")]
    //public async Task<IActionResult> LoginGoogle([FromBody] string idToken)
    //{
    //    try
    //    {
    //        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
    //        var email = payload.Email?.Trim().ToLower();

    //        var allowedDomains = new[] { "@stu.untar.ac.id", "@mhs.untar.ac.id", "@untar.ac.id" };
    //        bool isValid = allowedDomains.Any(domain => email.EndsWith(domain));

    //        if (!isValid)
    //            return Unauthorized(new { message = "Email bukan domain UNTAR" });

    //        return Ok(new
    //        {
    //            message = "Login Google berhasil",
    //            email = payload.Email,
    //            name = payload.Name,
    //            role = "mahasiswa"
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return Unauthorized(new { message = "Token tidak valid", error = ex.Message });
    //    }
    //}
}
