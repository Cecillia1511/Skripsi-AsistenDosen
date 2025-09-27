using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

[ApiController]
[Route("api/[controller]")]
public class DosenController : ControllerBase
{
    private readonly IConfiguration _config;
    public DosenController(IConfiguration config) => _config = config;

    [HttpGet("dropdown")]
    public IActionResult GetDropdown()
    {
        var result = new List<object>();
        using var con = new SqlConnection(_config.GetConnectionString("AsistenDosen"));
        using var cmd = new SqlCommand("SELECT NIP, NamaDosen FROM Dosen", con);

        try
        {
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = reader["NIP"].ToString(),
                    Label = $" {reader["NIP"]} - {reader["NamaDosen"]}"
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
