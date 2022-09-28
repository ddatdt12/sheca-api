using Microsoft.AspNetCore.Mvc;
using Sheca.Models;

namespace Sheca.Controllers
{
    public class UserController : Controller
    {
        public DataContext _context { get; set; }
        public UserController(DataContext context)
        {
            _context = context;
        }
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
