using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Models;
using Sheca.Services;

namespace Sheca.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public DataContext _context { get; set; }
        public IMapper _mapper { get; set; }

        public AuthController(IAuthService auth, DataContext context, IMapper mapper)
        {
            _auth = auth;
            _context = context;
            _mapper = mapper;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUser)
        {
            (User? user, string? token) = await _auth.Login(loginUser);
            return Ok(new { message = "Login successfully.", data = _mapper.Map<UserDto>(user), token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUser)
        {
            (User user, string token) = await _auth.Register(registerUser);
            return Ok(new { message = "Register successfully.", token });
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