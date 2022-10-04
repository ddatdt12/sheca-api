using Microsoft.AspNetCore.Mvc;
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
        public Task ForgotPassword(string emai);
        public Task<bool> FindUserByEmai(string email);
        public Task VerifyEmailToken(TokenDTO tokenDTO);
        public Task ResetPassword(RegisterUserDto UserDto);
        void VerifyResetPassword(string email, string value);
    }
}
