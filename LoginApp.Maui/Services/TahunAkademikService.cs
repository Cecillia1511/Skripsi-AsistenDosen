using System.Data.SqlClient;
using LoginApp.Maui.Models;
using Microsoft.Extensions.Configuration;

namespace LoginApp.Maui.Services
{
    public class TahunAkademikService : ITahunAkademikService
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration.GetConnectionString("AsistenDosen");

        public TahunAkademikService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<JadwalOption>> GetDropdownAsync()
        {
            var result = new List<JadwalOption>();

            await using var con = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(@"
                SELECT TahunAkademik_ID, Tahun + ' - ' + Semester AS Label
                FROM TahunAkademik
                ORDER BY Tahun, Semester", con);

            await con.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new JadwalOption
                {
                    Id = reader["TahunAkademik_ID"].ToString(),
                    Label = reader["Label"].ToString()
                });
            }

            return result;
        }
    }
}
