using Microsoft.AspNetCore.Mvc;
using LoginApp.Api.Models;
using LoginApp.Api.Data;
using System.Linq;

namespace LoginApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    StatusMessage = "Username and Password are required"
                });
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Username == login.Username
                                  && u.Password == login.Password);

            if (user == null)
            {
                return Unauthorized(new
                {
                    StatusCode = 401,
                    StatusMessage = "Invalid credentials or inactive user"
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                StatusMessage = "Login successful",
                Data = user
            });
        }
    }
}
