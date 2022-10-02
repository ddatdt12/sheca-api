using Microsoft.AspNetCore.Mvc;
using Sheca.Models;
using Microsoft.AspNetCore.Authorization;
using Sheca.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Sheca.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : Controller
    {
        public DataContext _context { get; set; }
        public UserController(DataContext context)
        {
            _context = context;
        }

        [Protect]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            return Ok(new
            {
                data = await _context.Users.OrderBy(u => u.Id).ToListAsync()
            });
        }
    }
}
