using Microsoft.EntityFrameworkCore;
using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services
{
    public class EventService : IEventService
    {
        public readonly DataContext _context;

        public EventService(DataContext context)
        {
            _context = context;
        }

        Task<Event> IEventService.Create(Event e)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<Event>> IEventService.Get(FilterEvent filter)
        {
            var query = _context.Events.AsQueryable();

            if (filter.FromDate.HasValue)
            {
                query = query.Where(e => e.EndTime > filter.FromDate.Value.Date);
            }
            if (filter.ToDate.HasValue)
            {
                var enDate = filter.ToDate.Value.Date.AddDays(1);
                query = query.Where(e => e.StartTime < enDate); 
            }

            return await query.ToListAsync();
        }

        Task<Event> IEventService.GetById(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
