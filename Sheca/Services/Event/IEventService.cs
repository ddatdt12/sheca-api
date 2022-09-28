using Sheca.Models;

namespace Sheca.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> Get();
        Task<Event> GetById(int Id);
        Task<Event> Create(Event e);

    }
}
