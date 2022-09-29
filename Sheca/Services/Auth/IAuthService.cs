using Sheca.Models;

namespace Sheca.Services.Auth
{
    public interface IAuthService
    {
        string CreateToken(User user);
        string ValidateToken(string token);
        void Login(User user);
    }
}
