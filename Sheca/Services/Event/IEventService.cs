using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> Get(string userId, FilterEvent filter, CancellationToken cancellationToken = default);
        Task<Event> GetById(int Id, CancellationToken cancellationToken = default);
        Task<Event> Create(CreateEventDto e, string userId,CancellationToken cancellationToken = default);
        Task Update(UpdateEventDto e, string userId, CancellationToken cancellationToken = default);
        Task Delete(string userId, DeleteEventDto dE, CancellationToken cancellationToken = default);
    }
}
