namespace LoginApp.Api.Models
{
    public class ResponseMHS
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public Mahasiswa Data { get; set; }
    }
}
