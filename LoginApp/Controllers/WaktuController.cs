using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LoginApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaktuController : ControllerBase
    {
        private readonly IConfiguration _config;
        public WaktuController(IConfiguration config) => _config = config;

        [HttpGet("dropdown")]
        public IActionResult GetDropdown()
        {
            var result = new List<object>();
            using var con = new SqlConnection(_config.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand("SELECT Waktu_ID, WaktuMulai, WaktuSelesai FROM Waktu", con);

            try
            {
                con.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        Id = reader["Waktu_ID"].ToString(),
                        Label = $"{reader["WaktuMulai"]} - {reader["WaktuSelesai"]}"
                    });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusMessage = "Error", Error = ex.Message });
            }
        }
    }
}
