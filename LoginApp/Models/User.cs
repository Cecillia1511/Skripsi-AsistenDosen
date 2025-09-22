using System;
using System.Text.Json.Serialization;

namespace LoginApp.Api.Models
{
    public class User
    {
        public int User_ID { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Role_ID { get; set; }         
    }

    public class UserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
