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
        private readonly ILogger<AuthController> _logger;
        public static User user = new User();
        private readonly IAuthService _auth;
        public DataContext _context { get; set; }

        public AuthController(ILogger<AuthController> logger, IAuthService auth, DataContext context)
        {
            _logger = logger;
            _auth = auth;
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO userRequest)
        {
            if(user.Email != userRequest.Email)
            {
                return BadRequest("User not found");
            }
            if (user.Password != userRequest.Password)
            {
                return BadRequest("Wrong password");
            }
            string token = _auth.CreateToken(user);
            return Ok(new
            {
                message = "Login successfully",
                token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userRequest)
        {
            var item = _context.Users.Where(u => u.Email.Trim().ToLower() == userRequest.Email.Trim().ToLower()).FirstOrDefault();
            if(item != null)
            {
                return BadRequest(new ApiException("Email have already existed!!!", 400));
            }
            user.Email = userRequest.Email;
            user.Password = userRequest.Password;
            string token = _auth.CreateToken(user);
            _context.Add(user);
            _context.SaveChanges();
            return Ok(new
            {
                message = "Register successfully",
                token = token
            });
        }
    }
}