using LoginApp.Maui.Models;

namespace LoginApp.Maui.Services
{
    public interface ITahunAkademikService
    {
        Task<List<JadwalOption>> GetDropdownAsync();
    }
}
