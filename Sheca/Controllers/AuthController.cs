using Microsoft.AspNetCore.Mvc;
using Sheca.Error;
using Sheca.Models;
using Sheca.Services;

namespace Sheca.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            _authService.Login("", "");
            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }
    }
}