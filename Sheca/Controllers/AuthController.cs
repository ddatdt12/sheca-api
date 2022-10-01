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
        private readonly IMapper _mapper;
        public DataContext _context { get; set; }

        public AuthController( IAuthService auth, IMapper mapper, DataContext context)
        {
            _auth = auth;
            _mapper = mapper;
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO loginUser)
        {
            (User? user, string? token) = _auth.FindUserByEmailAndPassword(loginUser.Email, loginUser.Password);
            if (user == null)
            {
                return NotFound(new ApiException("Email or password is wrong", 400));
            }
            return Ok(new
            {
                message = "Login successfully",
                token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO registerUser)
        {
            if (_auth.FindUserByEmai(registerUser.Email))
            {
                return BadRequest(new ApiException("Email have already existed!!!", 400));
            }
            User user = _mapper.Map<User>(registerUser);
            string token = _auth.CreateToken(user);
            _context.Add(user);
            _context.SaveChanges();
            return Ok(new
            {
                message = "Register successfully",
                token = token
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!_auth.FindUserByEmai(email))
            {
                return BadRequest("User not found.");
            }
            return Ok("You may now reset your password.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(UserDTO reUser)
        {
            if (!_auth.FindUserByEmai(reUser.Email))
            {
                return BadRequest("Invalid User");
            }
            User user = _mapper.Map<User>(reUser);
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok("Password successfully reset.");
        }
    }
}