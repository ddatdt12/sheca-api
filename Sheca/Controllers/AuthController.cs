using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _auth;
        public DataContext _context { get; set; }

        public AuthController(ILogger<AuthController> logger, IAuthService auth, DataContext context)
        {
            _logger = logger;
            _auth = auth;
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO loginUser)
        {
            var user = _context.Users.Where(u => u.Email.Trim().ToLower() == loginUser.Email.Trim().ToLower() && u.Password == loginUser.Password).FirstOrDefault();
            if (user == null)
            {
                return NotFound(new ApiException("Email or password is wrong", 400));
                    
            }
            string token = _auth.CreateToken(user);
            return Ok(new
            {
                message = "Login successfully",
                token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO registerUser)
        {
            var user = _context.Users.Where(u => u.Email.Trim().ToLower() == registerUser.Email.Trim().ToLower()).FirstOrDefault();
            if (user != null)
            {
                return BadRequest(new ApiException("Email have already existed!!!", 400));
            }
            var _user = new User();
            _user.Email = registerUser.Email;
            _user.Password = registerUser.Password;
            string token = _auth.CreateToken(_user);
            _context.Add(_user);
            _context.SaveChanges();
            return Ok(new
            {
                message = "Register successfully",
                token = token
            });
        }

        [HttpPost("forgot password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            return Ok("You may now reset your password.");
        }

        [HttpPost("reset password")]
        public async Task<IActionResult> ResetPassword(UserDTO user)
        {
            var _user = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (_user == null)
            {
                return BadRequest("Invalid User");
            }
            _user.Password = user.Password;
            _context.Users.Update(_user);
            await _context.SaveChangesAsync();
            return Ok("Password successfully reset.");
        }
    }
}