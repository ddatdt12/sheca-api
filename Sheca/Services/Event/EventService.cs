using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Models;

namespace Sheca.Services
{
    public class EventService : IEventService
    {
        public readonly DataContext _context;
        private readonly IMapper _mapper;

        public EventService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        async Task<Event> IEventService.Create(CreateEventDto e, string userId)
        {
            Event @event = _mapper.Map<Event>(e);
            @event.UserId = new Guid(userId);
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return @event;
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

        async Task IEventService.Delete(Guid id, string userId)
        {
            var e = await _context.Events.FindAsync(id);
            if (e == null || e.UserId.ToString() == userId)
            {
                throw new ApiException("Event not found", 404);
            }
            _context.Events.Remove(e);
            await _context.SaveChangesAsync();
        }

        Task<Event> IEventService.GetById(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
