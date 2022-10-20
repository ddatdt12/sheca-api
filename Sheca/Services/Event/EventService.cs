using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Helper;
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

        async Task<Event> IEventService.Create(CreateEventDto e, string userId, CancellationToken cancellationToken)
        {

            if (e.RecurringUnit == RecurringUnit.WEEK && (e.RecurringDetails == null || e.RecurringDetails.Count == 0))
            {
                throw new ApiException("With week recurring option, Please choose days of week recurs", 400);
            }
            Event @event = _mapper.Map<Event>(e);

            if (@event.RecurringInterval.HasValue)
            {
                @event.RecurringStart = @event.StartTime;
            }

            @event.UserId = new Guid(userId);
            _context.Events.Add(@event);
            await _context.SaveChangesAsync(cancellationToken);

            return @event;
        }

        async Task<IEnumerable<Event>> IEventService.Get(string userId, FilterEvent filter, CancellationToken cT)
        {
            var guidUserId = new Guid(userId);
            var query = _context.Events.Where(e => e.UserId == guidUserId);
            DateTime fromDate = filter?.FromDate?.Date ?? DateTime.MinValue;
            DateTime endDate = filter?.ToDate?.Date.AddDays(1) ?? fromDate.AddDays(365);
            var duration = (endDate - fromDate).TotalSeconds;
            var totalDays = (endDate - fromDate).TotalSeconds;
            var totalWeeks = (endDate - fromDate).TotalDays / 7;
            if (filter != null)
            {
                if (filter.FromDate.HasValue && filter.ToDate.HasValue)
                {
                    query = query.Where(e =>
                    (e.StartTime.Date >= fromDate && e.StartTime.Date <= endDate)
                    || (e.RecurringInterval != null && e.RecurringInterval != 0 && e.StartTime.Date < fromDate
                    &&
                   (e.RecurringStart < endDate && (!e.RecurringEnd.HasValue || e.RecurringEnd > fromDate))
                    ));
                }
                else
                {
                    if (filter.FromDate.HasValue)
                    {
                        query = query.Where(e => e.EndTime > fromDate);
                    }
                    if (filter.ToDate.HasValue)
                    {
                        query = query.Where(e => e.StartTime < endDate);
                    }
                }
            }

            var listEvents = await query.ToListAsync(cT);
            var finalEvents = new List<Event>();
            listEvents.ForEach(e =>
            {
                if (e.RecurringInterval.HasValue && duration != 0 && e.RecurringStart.HasValue)
                {
                    var sameEvents = new List<Event>();
                    var removedEvents = e.ExceptDates.Split(";").ToDictionary(t => t, t => t);
                    var startDateTemp = (DateTime)e.RecurringStart > fromDate ? (DateTime)e.RecurringStart : fromDate;
                    var endDateTemp = (e.RecurringEnd.HasValue && e.RecurringEnd < endDate) ? (DateTime)e.RecurringEnd : endDate;
                    var timeSpan = e.EndTime - e.StartTime;

                    //WEEK recurring
                    if (e.RecurringUnit == RecurringUnit.WEEK && e.RecurringDetails != null)
                    {
                        List<DayOfWeek>? dayOfWeeksRecurrings = e.RecurringDetails.Split(';').Select(d => (DayOfWeek)int.Parse(d)).ToList();
                        foreach (var dayOfWeek in dayOfWeeksRecurrings)
                        {
                            startDateTemp = Utils.GetNextWeekday(startDateTemp, dayOfWeek);

                            var recurringCount = Math.Ceiling((endDateTemp - startDateTemp).TotalDays / (7 * (int)e.RecurringInterval));
                            for (int dI = 0; dI < recurringCount; dI++)
                            {
                                var duplicateE = e.Clone();
                                duplicateE.Id = Guid.NewGuid();
                                duplicateE.StartTime = Utils.GetNextWeekday(startDateTemp, dayOfWeek).AddDays(7 * (int)e.RecurringInterval * dI);
                                duplicateE.EndTime = duplicateE.StartTime + timeSpan;
                                duplicateE.CloneEventId = e.Id;
                                if (duplicateE.StartTime >= startDateTemp && duplicateE.EndTime <= endDateTemp && !removedEvents.ContainsKey(TimeSpan.FromTicks(duplicateE.StartTime.Ticks).TotalSeconds.ToString()))
                                {
                                    sameEvents.Add(duplicateE);
                                }
                            }
                        }
                    }
                    else
                    {
                        var consideredDate = e.StartTime;
                        if (consideredDate < fromDate)
                        {
                            var times = (fromDate - consideredDate).TotalDays / (e.RecurringInterval.Value * (e.RecurringUnit == RecurringUnit.DAY ? 1 : 7));
                            UpdateDateTime(ref consideredDate, (int)Math.Ceiling(times) * e.RecurringInterval.Value, (RecurringUnit)e.RecurringUnit!);
                        }

                        while (consideredDate <= endDateTemp)
                        {
                            var duplicateE = e.Clone();
                            duplicateE.Id = Guid.NewGuid();
                            duplicateE.StartTime = consideredDate;
                            duplicateE.EndTime = consideredDate + timeSpan;
                            duplicateE.CloneEventId = e.Id;
                            if ((duplicateE.StartTime >= startDateTemp && duplicateE.EndTime <= endDateTemp) && !removedEvents.ContainsKey(TimeSpan.FromTicks(duplicateE.StartTime.Ticks).TotalSeconds.ToString()))
                            {
                                sameEvents.Add(duplicateE);
                            }

                            UpdateDateTime(ref consideredDate, (int)e.RecurringInterval, (RecurringUnit)e.RecurringUnit!);
                        }
                    }
                    finalEvents.AddRange(sameEvents);
                }
                else
                {
                    finalEvents.Add(e);
                }
            });


            return finalEvents.Where(e => e.StartTime >= fromDate && e.EndTime <= endDate).OrderBy(e => e.StartTime).ToList();
        }
        private void UpdateDateTime(ref DateTime current, int value, RecurringUnit unit)
        {
            switch (unit)
            {
                case RecurringUnit.DAY:
                    current = current.AddDays(value);
                    break;
                case RecurringUnit.WEEK:
                    break;
                case RecurringUnit.MONTH:
                    current = current.AddMonths(value);
                    break;
                default:
                    break;
            }
        }
        Task<Event> IEventService.GetById(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IEventService.Delete(string userId, DeleteEventDto dE, CancellationToken cancellationToken)
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
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    await _context.Events.DeleteByKeyAsync(cancellationToken, mainEventId);
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
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    await _context.Events.DeleteByKeyAsync(cancellationToken, mainEventId);
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
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(UpdateEventDto upE, string userId, CancellationToken cancellationToken = default)
        {
            if (!upE.CloneEventId.HasValue && !upE.Id.HasValue && !upE.BaseEventId.HasValue && (upE.BaseEventId.HasValue || !upE.BeforeStartTime.HasValue))
            {
                throw new ApiException("Bad request", 400);
            }
            var eventId = upE.CloneEventId ?? upE.Id;

            string typeId = upE.CloneEventId.HasValue ? "CloneId" : (upE.BaseEventId.HasValue ? "BaseId" : "Main");

            var currentEvent = await _context.Events.FindAsync(eventId);
            if (currentEvent == null || currentEvent.UserId.ToString() != userId)
            {
                throw new ApiException("Event not exist!", 404);
            }

            if (typeId != "Main")
            {
                if (upE.StartTime.HasValue && !upE.BeforeStartTime.HasValue)
                {
                    throw new ApiException("Please provide before start time!", 400);
                }


                if (upE.TargetType == TargetType.ALL)
                {
                    throw new ApiException("This target type is not supported", 400);
                }
                if (upE.TargetType == TargetType.THIS)
                {
                    if (typeId == "CloneId")
                    {
                        if (upE.StartTime.HasValue && upE.StartTime != currentEvent.StartTime && upE.EndTime != currentEvent.EndTime)
                        {
                            currentEvent.ExceptDates += (string.IsNullOrEmpty(currentEvent.ExceptDates) ? "" : ";") + $"{TimeSpan.FromTicks(upE.BeforeStartTime!.Value.Ticks).TotalSeconds}";
                        }

                        var newEv = currentEvent.Clone();
                        newEv.Id = Guid.NewGuid();
                        _mapper.Map(upE, newEv);
                        newEv.BaseEventId = eventId;
                        await _context.AddAsync(newEv);
                    }
                    else
                    {
                        _mapper.Map(upE, currentEvent);
                    }

                    await _context.SaveChangesAsync();
                }
                else //  TargetType.THIS_AND_FOLLOWING
                {
                    if (!upE.BeforeStartTime.HasValue)
                    {
                        throw new ApiException("Please provide start time of this event", 400);
                    }

                    if (!upE.StartTime.HasValue || upE.StartTime?.Date == upE.BeforeStartTime?.Date)
                    {
                        var events = await _context.Events.Where(e => e.BaseEventId == eventId).ToListAsync();

                        _mapper.Map(upE, currentEvent);
                        foreach (var @event in events)
                        {
                            _mapper.Map(upE, @event);
                        }

                        await _context.BulkSaveChangesAsync();
                    }
                    else
                    {
                        if (upE.BeforeStartTime == currentEvent.StartTime)
                        {
                            var events = await _context.Events.Where(e => e.BaseEventId == eventId).ToListAsync();

                            _mapper.Map(upE, currentEvent);
                            foreach (var @event in events)
                            {
                                _mapper.Map(upE, @event);
                            }

                            await _context.BulkSaveChangesAsync(cancellationToken);
                        }
                        else
                        {
                            if (currentEvent.RecurringStart.HasValue)
                            {
                                currentEvent.RecurringEnd = upE.BeforeStartTime!.Value.AddSeconds(-1 * currentEvent?.RecurringInterval ?? 0);

                                var newEvent = currentEvent!.Clone();
                                _mapper.Map(upE, newEvent);
                                await _context.Events.AddAsync(newEvent);
                                await _context.BulkSaveChangesAsync(cancellationToken);
                            }
                        }
                    }
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
