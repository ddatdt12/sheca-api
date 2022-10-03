using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services
{
    public interface IAuthService
    {
        string CreateToken(User user);
        string ValidateToken(string token);
        public Task<(User, string token)> Login(LoginUserDto UserDto);
        public Task Register(RegisterUserDto userDTO);
        public Task<bool> FindUserByEmai(string email);
        public Task<> VerifyEmailToken();
        public Task ResetPassword(RegisterUserDto UserDto);
    }
}
