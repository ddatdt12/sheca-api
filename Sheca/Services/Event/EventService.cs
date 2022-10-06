using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Models;
using static Sheca.Common.Enum;

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

        async Task<IEnumerable<Event>> IEventService.Get(string userId, FilterEvent filter)
        {
            var guidUserId = new Guid(userId);
            var query = _context.Events.Where(e => e.UserId == guidUserId);
            DateTime? fromDate = filter?.FromDate?.Date ?? DateTime.MinValue;
            DateTime? endDate = filter?.ToDate?.Date.AddDays(1) ?? DateTime.MaxValue;
            var duration = (endDate - fromDate)?.TotalSeconds ?? 0;

            if (fromDate.HasValue && endDate.HasValue)
            {
                query = query.Where(e =>
                (e.StartTime.Date >= fromDate && e.StartTime.Date <= endDate)
                || (e.RecurringInterval != null && e.RecurringInterval != 0 && e.StartTime.Date < fromDate && duration / e.RecurringInterval >= 1)
                );
            }
            else
            {
                if (fromDate.HasValue)
                {
                    query = query.Where(e => e.EndTime > fromDate);
                }
                if (endDate.HasValue)
                {
                    query = query.Where(e => e.StartTime < endDate);
                }
            }
            var listEvents = await query.ToListAsync();
            var finalEvents = new List<Event>();
            listEvents.ForEach(e =>
            {
                if (e.RecurringInterval.HasValue && duration != 0 && fromDate.HasValue && endDate.HasValue)
                {
                    var maxTimes = duration / e.RecurringInterval;
                    var sameEvents = new List<Event>();
                    var removedEvents = e.ExceptDates.Split(";").ToDictionary(t => t, t => t);
                    if (e.StartTime.Date >= fromDate?.Date)
                    {
                        sameEvents.Add(e);
                        for (int i = 1; i <= maxTimes; i++)
                        {
                            if (e.StartTime.AddSeconds(i * (int)e.RecurringInterval) > e.RecurringEnd)
                            {
                                break;
                            }
                            var duplicateE = e.Clone();
                            duplicateE.Id = Guid.NewGuid();
                            duplicateE.StartTime = e.StartTime.AddSeconds(i * (int)e.RecurringInterval);
                            duplicateE.EndTime = e.EndTime.AddSeconds(i * (int)e.RecurringInterval);
                            duplicateE.CloneEventId = e.Id;

                            if (!removedEvents.ContainsKey(TimeSpan.FromTicks(duplicateE.StartTime.Ticks).TotalSeconds.ToString()))
                            {
                                sameEvents.Add(duplicateE);
                            }
                        }
                    }
                    else
                    {
                        var startIndex = (int)Math.Ceiling((fromDate?.Date - e.StartTime.Date)?.TotalSeconds / duration ?? 0);
                        for (int i = startIndex; i <= startIndex + maxTimes; i++)
                        {
                            if (e.StartTime.AddSeconds(i * (int)e.RecurringInterval) > e.RecurringEnd)
                            {
                                break;
                            }
                            var duplicateE = e.Clone();
                            duplicateE.Id = Guid.NewGuid();
                            duplicateE.StartTime = e.StartTime.AddSeconds(i * (int)e.RecurringInterval);
                            duplicateE.EndTime = e.EndTime.AddSeconds(i * (int)e.RecurringInterval);
                            duplicateE.CloneEventId = e.Id;

                            if (!removedEvents.ContainsKey(TimeSpan.FromTicks(duplicateE.StartTime.Ticks).TotalSeconds.ToString()))
                            {
                                sameEvents.Add(duplicateE);
                            }
                        }
                    }

                    finalEvents.AddRange(sameEvents);
                }
                else
                {
                    finalEvents.Add(e);
                }
            });


            return finalEvents.OrderBy(e => e.StartTime).ToList();
        }

        Task<Event> IEventService.GetById(int Id)
        {
            throw new NotImplementedException();
        }

        async Task IEventService.Delete(string userId, DeleteEventDto dE)
        {
            if (!dE.CloneEventId.HasValue && !dE.Id.HasValue && !dE.BaseEventId.HasValue && (dE.BaseEventId.HasValue || !dE.StartTime.HasValue))
            {
                throw new ApiException("Bad request", 400);
            }

            Guid mainEventId = Guid.Empty;
            if (dE.CloneEventId.HasValue)
            {
                var ev = await _context.Events.FindAsync(dE.CloneEventId);
                if (ev == null || !dE.StartTime.HasValue || ev.UserId.ToString() != userId)
                {
                    throw new ApiException("Invalid event!", 404);
                }
                mainEventId = (Guid)dE.CloneEventId;
                ev.ExceptDates += (string.IsNullOrEmpty(ev.ExceptDates) ? "" : ";") + $"{TimeSpan.FromTicks(dE.StartTime.Value.Ticks).TotalSeconds}";
                if (dE.TargetType == TargetType.THIS)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.Events.DeleteByKeyAsync(mainEventId);
                }

                return;
            }
            else
            if (dE.BaseEventId.HasValue)
            {
                if (dE.Id.HasValue)
                {
                    throw new ApiException("Insufficient data", 400);
                }
                mainEventId = (Guid)dE.BaseEventId;

                var ev = await _context.Events.FindAsync(dE.Id);
                if (ev == null || ev.UserId.ToString() != userId)
                {
                    throw new ApiException("Invalid event!", 404);
                }

                if (dE.TargetType == TargetType.THIS)
                {
                    _context.Events.Remove(ev);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.Events.DeleteByKeyAsync(mainEventId);
                }

                return;
            }

            var e = await _context.Events.FindAsync(dE.Id);
            if (e == null || e.UserId.ToString() != userId)
            {
                throw new ApiException("Event not found", 404);
            }

            if (dE.TargetType == TargetType.THIS)
            {
                e.ExceptDates += (string.IsNullOrEmpty(e.ExceptDates) ? "" : ";") + $"{TimeSpan.FromTicks(dE.StartTime!.Value.Ticks).TotalSeconds}";
            }
            else
            {
                _context.Events.Remove(e);
            }
            await _context.SaveChangesAsync();
        }

        public async Task Update(UpdateEventDto upE, string userId)
        {
            if (!upE.CloneEventId.HasValue && !upE.Id.HasValue && !upE.BaseEventId.HasValue && (upE.BaseEventId.HasValue || !upE.BeforeStartTime.HasValue))
            {
                throw new ApiException("Bad request", 400);
            }

            if (upE.CloneEventId.HasValue)
            {
                var ev = await _context.Events.FindAsync(upE.CloneEventId);
                if (ev == null || ev.UserId.ToString() != userId)
                {
                    throw new ApiException("Event not exist!", 404);
                }

                if (upE.StartTime.HasValue && !upE.BeforeStartTime.HasValue)
                {
                    throw new ApiException("Please provide before start time!", 400);
                }

                if (upE.TargetType == TargetType.THIS)
                {
                    if (upE.StartTime.HasValue && upE.StartTime != ev.StartTime && upE.EndTime != ev.EndTime)
                    {
                        ev.ExceptDates += (string.IsNullOrEmpty(ev.ExceptDates) ? "" : ";") + $"{TimeSpan.FromTicks(upE.BeforeStartTime!.Value.Ticks).TotalSeconds}";
                    }

                    var newEv = ev.Clone();
                    _mapper.Map(upE, newEv);
                    newEv.BaseEventId = upE.CloneEventId;
                    await _context.AddAsync(newEv);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (upE.StartTime?.Date == upE.BeforeStartTime?.Date)
                    {
                        var events = await _context.Events.Where(e => e.Id == upE.CloneEventId || e.BaseEventId == upE.CloneEventId).ToListAsync();

                        foreach (var @event in events)
                        {
                            _mapper.Map(upE, @event);
                        }

                        await _context.BulkSaveChangesAsync();
                    }
                    else
                    {
                        
                    }
                }
                return;
            }
            else if (upE.BaseEventId.HasValue)
            {
                if (upE.Id.HasValue)
                {
                    throw new ApiException("Insufficient data", 400);
                }

                var ev = await _context.Events.FindAsync(upE.Id);
                if (ev == null || ev.UserId.ToString() != userId)
                {
                    throw new ApiException("Invalid event!", 404);
                }

                if (upE.TargetType == TargetType.THIS)
                {
                    _mapper.Map(upE, ev);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newEv = new Event();
                    _mapper.Map(upE, newEv);
                    newEv.Id = Guid.Empty;
                    await _context.Events.Where(e => e.Id == upE.BaseEventId || e.BaseEventId == upE.BaseEventId).UpdateFromQueryAsync(e => newEv);
                }
                return;
            }

            var e = await _context.Events.FindAsync(upE.Id);
            if (e == null || e.UserId.ToString() != userId)
            {
                throw new ApiException("Event not found", 404);
            }

            if (upE.TargetType == TargetType.THIS)
            {
                e.ExceptDates += (string.IsNullOrEmpty(e.ExceptDates) ? "" : ";") + $"{TimeSpan.FromTicks(upE.StartTime!.Value.Ticks).TotalSeconds}";
            }
            else
            {
                _context.Events.Remove(e);
            }
            await _context.SaveChangesAsync();
        }
    }
}
