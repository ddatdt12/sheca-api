using Sheca.Models;

namespace Sheca.Services
{
    public interface IUserService
    {
        Task<User?> GetById(Guid id);
    }
}
