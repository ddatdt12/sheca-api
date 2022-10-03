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
        public Task<(User, string token)> VerifyEmailToken(TokenDTO tokenDTO);
        public Task VerifyResetPassword(string email, string code);
        public Task ResetPassword(RegisterUserDto UserDto);
    }
}
