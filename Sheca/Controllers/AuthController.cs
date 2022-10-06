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

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO loginUser)
        {
            (User? user, string? token) = await _auth.Login(loginUser);
            return Ok(new { message = "Login successfully.", data = user, token });
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