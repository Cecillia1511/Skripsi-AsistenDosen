using LoginApp.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LoginApp.Api.Controllers
{
    internal class LoginResponseMHS : ModelStateDictionary
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public Mahasiswa Data { get; internal set; }
    }
}