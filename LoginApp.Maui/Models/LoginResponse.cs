namespace LoginApp.Maui.Models
{
    public class LoginResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public User Data { get; set; }  
    }
}
