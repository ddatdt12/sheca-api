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

        public AuthController(ILogger<AuthController> logger, IAuthService auth)
        {
            _logger = logger;
            _auth = auth;
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
                data = user,
                token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userRequest)
        {
            user.Email = userRequest.Email;
            user.Password = userRequest.Password;
            string token = _auth.CreateToken(user);
            return Ok(new
            {
                data = user,
                token = token
            });
        }
    }
}