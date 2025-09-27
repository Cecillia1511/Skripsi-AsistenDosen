using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]
public class TahunAkademikController : ControllerBase
{
    private readonly IConfiguration _config;
    public TahunAkademikController(IConfiguration config) => _config = config;

    [HttpGet("dropdown")]
    public IActionResult GetDropdown()
    {
        var result = new List<object>();
        using var con = new SqlConnection(_config.GetConnectionString("AsistenDosen"));
        using var cmd = new SqlCommand("SELECT TahunAkademik_ID, Tahun, Semester FROM TahunAkademik", con);

        try
        {
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = reader["TahunAkademik_ID"].ToString(),
                    Label = $" {reader["Semester"]} - {reader["Tahun"]}"
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
