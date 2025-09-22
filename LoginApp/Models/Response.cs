namespace LoginApp.Api.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public User Data { get; set; }
    }
}
