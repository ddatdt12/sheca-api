using Microsoft.EntityFrameworkCore;
using Sheca.Models;

namespace Sheca.Services
{
    public class EventService : IEventService
    {
        public readonly DataContext _context;

        public EventService(DataContext context)
        {
            _context=context;
        }

        Task<Event> IEventService.Create(Event e)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<Event>> IEventService.Get()
        {
            return await _context.Events.ToListAsync();
        }

        Task<Event> IEventService.GetById(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
