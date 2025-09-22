namespace LoginApp.Maui.Models
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Gmail { get; set; } = string.Empty;
    }
}
