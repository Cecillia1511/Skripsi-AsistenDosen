using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

[ApiController]
[Route("api/[controller]")]
public class HariController : ControllerBase
{
    private readonly IConfiguration _config;
    public HariController(IConfiguration config) => _config = config;

    [HttpGet("dropdown")]
    public IActionResult GetDropdown()
    {
        var result = new List<object>();
        using var con = new SqlConnection(_config.GetConnectionString("AsistenDosen"));
        using var cmd = new SqlCommand("SELECT Hari_ID, NamaHari FROM Hari", con);

        try
        {
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = reader["Hari_ID"].ToString(),
                    Label = reader["NamaHari"].ToString()
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
