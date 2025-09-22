namespace LoginApp.Maui.Models
{
    public class LoginResponseMHS
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public Mahasiswa? Data { get; set; }

    }
}
