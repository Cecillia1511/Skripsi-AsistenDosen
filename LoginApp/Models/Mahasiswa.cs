using System.Text.Json.Serialization;

namespace LoginApp.Api.Models
{
    public class Mahasiswa
    {
        public int Id { get; set; } 
        public string Nama { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Mahasiswa";
        public DateTime TanggalDaftar { get; set; } = DateTime.Now;
    }

    public class RegisterMahasiswaRequest
    {
        public string Nama { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;  
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}
