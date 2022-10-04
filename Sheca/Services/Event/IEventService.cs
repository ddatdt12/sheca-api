using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> Get(string userId, FilterEvent filter);
        Task<Event> GetById(int Id);
        Task<Event> Create(CreateEventDto e, string userId);
        Task Delete(string userId, DeleteEventDto dE);
    }
}
