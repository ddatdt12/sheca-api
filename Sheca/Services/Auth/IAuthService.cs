using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services.Auth
{
    public interface IAuthService
    {
        string CreateToken(User user);
        string ValidateToken(string token);
        public Task<(User, string token)> Login(UserDTO userDTO);
        public Task Register(UserDTO userDTO);
        public Task<bool> FindUserByEmai(string email);
        public Task ResetPassword(UserDTO userDTO);
        public Task<> VerifyEmailToken
    }
}
