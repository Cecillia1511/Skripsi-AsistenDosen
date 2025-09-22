using LoginApp.Maui.Models;

namespace LoginApp.Maui.Services;

public interface ILoginRepository
{
    Task<User> Login(string username, string password);

}
