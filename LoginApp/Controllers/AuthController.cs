using BCrypt.Net;
using LoginApp.Api.Models;  // Pastikan ini sesuai namespace model kamu
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("RegisterMahasiswa")]
    public IActionResult RegisterMahasiswa([FromBody] RegisterMahasiswaRequest request)
    {
        var result = new RegisterResponse { Success = false, Message = "" };

        using var con = new SqlConnection(_configuration.GetConnectionString("AsistenDosen"));
        try
        {
            string nama = request.Nama?.Trim() ?? "";
            string username = request.Username?.Trim() ?? "";
            string email = request.Email?.Trim().ToLower() ?? "";
            string password = request.Password ?? "";

            // Validasi
            if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                result.Message = "Semua field harus diisi!";
                return BadRequest(result);
            }

            if (password.Length <= 4)
            {
                result.Message = "Password minimal 4 karakter!";
                return BadRequest(result);
            }

            if (!email.EndsWith("@stu.untar.ac.id"))
            {
                result.Message = "Email harus berakhiran @untar.ac.id!";
                return BadRequest(result);
            }

            if (!Regex.IsMatch(username, @"^\d{9}$"))
            {
                result.Message = "NIM harus tepat 9 digit angka!";
                return BadRequest(result);
            }

            if (!username.StartsWith("825") && !username.StartsWith("535"))
            {
                result.Message = "NIM harus dimulai dengan 825 atau 535 (khusus FTI Untar)!";
                return BadRequest(result);
            }

            con.Open();

            var checkCmd = new SqlCommand(@"
                SELECT COUNT(*) FROM [User] WHERE Username = @username OR Email = @email", con);
            checkCmd.Parameters.AddWithValue("@username", username);
            checkCmd.Parameters.AddWithValue("@email", email);

            int existingCount = (int)checkCmd.ExecuteScalar();
            if (existingCount > 0)
            {
                result.Message = "Username (NIM) atau email sudah terdaftar!";
                return BadRequest(result);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var insertCmd = new SqlCommand(@"
                INSERT INTO [User] (Nama, Username, Password, Email, Role_ID) 
                VALUES (@nama, @username, @password, @email, @roleId)", con);
            insertCmd.Parameters.AddWithValue("@nama", nama);
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@password", hashedPassword);
            insertCmd.Parameters.AddWithValue("@email", email);
            insertCmd.Parameters.AddWithValue("@roleId", 5);

            int rowsAffected = insertCmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                result.Success = true;
                result.Message = $"Akun mahasiswa {nama} berhasil dibuat! Silakan login dengan NIM {username}.";
                return Ok(result);
            }
            else
            {
                result.Message = "Gagal menyimpan data. Coba lagi.";
                return StatusCode(500, result);
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"[REGISTER MAHASISWA] SQL Error: {ex.Message}");
            result.Message = "Error database. Pastikan NIM unik.";
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[REGISTER MAHASISWA] Unexpected Error: {ex.Message}");
            result.Message = "Terjadi kesalahan server. Coba lagi nanti.";
            return StatusCode(500, result);
        }
    }
}
