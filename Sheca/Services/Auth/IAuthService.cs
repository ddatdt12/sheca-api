using Sheca.Models;

namespace Sheca.Services.Auth
{
    public interface IAuthService
    {
        string CreateToken(User user);
        string ValidateToken(string token);
        public (User?, string? token) FindUserByEmailAndPassword(string email, string password);
        bool FindUserByEmai(string emai);
    }
}
