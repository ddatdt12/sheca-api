using Sheca.Models;

namespace Sheca.Services
{
    public class UserService : IUserService
    {
        private DataContext _context { get; set; }
        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<User?> GetById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
