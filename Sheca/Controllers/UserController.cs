using Microsoft.AspNetCore.Mvc;
using Sheca.Models;
using Microsoft.AspNetCore.Authorization;

namespace Sheca.Controllers
{
    public class UserController : Controller
    {
        public DataContext _context { get; set; }
        public UserController(DataContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            return Ok(new
            {
                data = _context.Users.OrderBy(u => u.Id).ToList()
            });
        }
    }
}
