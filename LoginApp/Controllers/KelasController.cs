using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LoginApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KelasController : ControllerBase
    {
        private readonly IConfiguration _config;
        public KelasController(IConfiguration config) => _config = config;

        [HttpGet("dropdown")]
        public IActionResult GetDropdown()
        {
            var result = new List<object>();
            using var con = new SqlConnection(_config.GetConnectionString("AsistenDosen"));
            using var cmd = new SqlCommand("SELECT Kelas_ID, NamaKelas FROM Kelas", con);

            try
            {
                con.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        Id = reader["Kelas_ID"].ToString(),
                        Label = reader["NamaKelas"].ToString()
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
