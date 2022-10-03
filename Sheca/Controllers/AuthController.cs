using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Models;
using Sheca.Services.Auth;

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
        public async Task<IActionResult> Login(UserDTO loginUser)
        {
            (User? user, string? token) = await _auth.Login(loginUser);
            return Ok(new { message = "Login successfully.", data = user, token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO registerUser)
        {
            await _auth.Register(registerUser);
            return Ok(new { message = "Send email verification successfully" });
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
        public async Task<IActionResult> ResetPassword(UserDTO reUser)
        {
            await _auth.ResetPassword(reUser);
            return Ok("Reset Password successfully.");
        }
    }
}