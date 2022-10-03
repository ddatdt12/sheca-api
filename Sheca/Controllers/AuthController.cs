using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sheca.Dtos;
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

        public AuthController(IAuthService auth, DataContext context)
        {
            _auth = auth;
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUser)
        {
            (User? user, string? token) = await _auth.Login(loginUser);
            return Ok(new { message = "Login successfully.", data = user, token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUser)
        {
            await _auth.Register(registerUser);
            return Ok(new { message = "Send email verification successfully" });
        }

        [HttpPost("verify-account")]
        public async Task<IActionResult> VerifyEmailToken(TokenDTO tokenDTO)
        {
            (User? user, string? token) = await _auth.VerifyEmailToken(tokenDTO);
            return Ok(new { message = "Verify account successfully!", data = user, token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!(await _auth.FindUserByEmai(email)))
            {
                return BadRequest("User not found.");
            }
            else
            {
                return Ok("You may now reset your password.");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(RegisterUserDto reUser)
        {
            await _auth.ResetPassword(reUser);
            return Ok("Reset Password successfully.");
        }
    }
}