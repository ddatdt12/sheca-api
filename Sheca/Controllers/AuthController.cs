using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sheca.Dtos;
using Sheca.Dtos.User;
using Sheca.Models;
using Sheca.Services;

namespace Sheca.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public DataContext _context { get; set; }
        public IMapper _mapper { get; set; }

        public AuthController(IAuthService auth, DataContext context, IMapper mapper)
        {
            _auth = auth;
            _context = context;
            _mapper=mapper;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUser)
        {
            (User? user, string? token) = await _auth.Login(loginUser);
            return Ok(new { message = "Login successfully.", data = _mapper.Map<UserDto>(user), token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUser)
        {
            await _auth.Register(registerUser);
            return Ok("Send email verification successfully");
        }

        [HttpPost("verify-account")]
        public async Task<IActionResult> VerifyEmailToken([FromBody] TokenDTO tokenDTO)
        {
            await _auth.VerifyEmailToken(tokenDTO);
            return Ok(new { message = "Verify account successfully!", });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            await _auth.ForgotPassword(email);
            return Ok("Please check the code in your email. This code consists of 4 numbers.");
        }

        [HttpPost("verify-code-repassword")]
        public IActionResult VerifyResetPassword([FromBody] TokenForgotPasswordDto user)
        {
            string token = _auth.VerifyResetPassword(user.Email, user.Code);
            return Ok(new {message = "Verify code successfully!", token});
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(TokenResetPasswordDto reUser)
        {
            await _auth.ResetPassword(reUser);
            return Ok("Reset Password successfully.");
        }
    }
}