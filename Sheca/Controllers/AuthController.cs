using Microsoft.AspNetCore.Mvc;
using Sheca.Models;

namespace Sheca.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {


        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "login")]
        public IActionResult Login()
        {
            return Ok();
        }

        [HttpPost(Name = "register")]
        public IActionResult Register()
        {
            return Ok();
        }
    }
}