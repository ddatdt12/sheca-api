using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Extensions;
using Sheca.Helper;
using Sheca.Models;
using static Sheca.Common.Enum;

namespace Sheca.Services
{
    public partial class EventService : IEventService
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

            if (e.RecurringUnit == RecurringUnit.WEEK)
            {
                if (e.RecurringDetails == null || e.RecurringDetails.Count == 0)
                {
                    throw new ApiException("With week recurring option, Please choose days of week recurs", 400);
                }
                DateTime minNextDate = DateTime.MaxValue;
                var timeSpan = e.EndTime - e.StartTime;
                foreach (var dOW in e.RecurringDetails)
                {
                    var nextDOW = Utils.GetNextWeekday(e.StartTime, dOW);
                    if (nextDOW < minNextDate)
                    {
                        minNextDate = nextDOW;
                    }
                }
                e.StartTime = minNextDate;
                e.EndTime = e.StartTime + timeSpan;
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
        //private void HandleFilterDate<T>(IQueryable<T> query , DateTime fromDate, DateTime){

        //}
        async Task<IEnumerable<Event>> IEventService.Get(string userId, FilterEvent filter, CancellationToken cT)
        {
            var guidUserId = new Guid(userId);
            var query = _context.Events.Where(e => e.UserId == guidUserId);
            var courseQuery = _context.Courses.Where(e => e.UserId == guidUserId);
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

                    courseQuery = courseQuery.Where(e =>
                    !(e.StartDate.Date > endDate && e.EndDate.Date < fromDate));
                }
                else
                {
                    if (filter.FromDate.HasValue)
                    {
                        query = query.Where(e => e.EndTime > fromDate);
                        courseQuery = courseQuery.Where(e => e.StartDate >= fromDate);
                    }
                    if (filter.ToDate.HasValue)
                    {
                        query = query.Where(e => e.StartTime <= endDate);
                    }
                }
            }

            var finalEvents = new List<Event>();

            if (filter?.Type != TypeQuery.EVENT)
            {
                var listCourses = await courseQuery.ToListAsync(cT);
                listCourses.ForEach(c =>
                {
                    var startDateTemp = c.StartDate > fromDate ? c.StartDate : fromDate;
                    var endDateTemp = c.EndDate < endDate ? c.EndDate : endDate;
                    var timeSpan = c.EndTime - c.StartTime;
                    var dayoffs = c.GetOffDaysList();

                    //Tìm ngày hợp lệ gần nhất
                    foreach (var day in c.GetDayOfWeeks())
                    {
                        var nextDate = Utils.GetNextWeekday(startDateTemp, day);
                        while (nextDate < endDateTemp)
                        {
                            var startTime = nextDate.Date.AddSeconds(c.StartTime);

                            if (!dayoffs.Contains(startTime))
                            {
                                finalEvents.Add(new Event
                                {
                                    Id = Guid.NewGuid(),
                                    Title = c.Title,
                                    Description = c.Description,
                                    CourseId = c.Id,
                                    StartTime = startTime,
                                    EndTime = startTime.AddSeconds(timeSpan),
                                    NotiBeforeTime = c.NotiBeforeTime,
                                    ColorCode = c.ColorCode,
                                    UserId = c.UserId,
                                });
                            }

                            nextDate = nextDate.AddDays(7);
                        }
                    }
                });
            }

            if (filter?.Type != TypeQuery.COURSE)
            {
                var listEvents = await query.ToListAsync(cT);

                listEvents.ForEach(e =>
                {
                    if (e.RecurringInterval.HasValue && duration != 0 && e.RecurringStart.HasValue)
                    {
                        var sameEvents = new List<Event>();
                        var removedEvents = e.GetExceptDates().ToDictionary(t => t, t => t);
                        var startDateTemp = (DateTime)e.RecurringStart > fromDate ? (DateTime)e.RecurringStart : fromDate;
                        var endDateTemp = (e.RecurringEnd.HasValue && e.RecurringEnd < endDate) ? (DateTime)e.RecurringEnd : endDate;
                        var timeSpan = e.EndTime - e.StartTime;

                        //WEEK recurring
                        if (e.RecurringUnit == RecurringUnit.WEEK && e.RecurringDetails != null)
                        {
                            List<DayOfWeek>? dayOfWeeksRecurrings = e.GetRecurringDetails();
                            foreach (var dayOfWeek in dayOfWeeksRecurrings)
                            {
                                startDateTemp = Utils.GetNextWeekday(startDateTemp, dayOfWeek);

                                var recurringCount = Math.Ceiling((endDateTemp - startDateTemp).TotalDays / (7 * (int)e.RecurringInterval));
                                for (int dI = 0; dI < recurringCount; dI++)
                                {
                                    var duplicateE = e.Clone();
                                    duplicateE.StartTime = Utils.GetNextWeekday(startDateTemp, dayOfWeek).AddDays(7 * (int)e.RecurringInterval * dI);
                                    duplicateE.Id = e.StartTime == duplicateE.StartTime ? e.Id : Guid.NewGuid();
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
            }

            return finalEvents.Where(e => e.StartTime >= fromDate && e.EndTime <= endDate).OrderBy(e => e.StartTime).ToList();
        }


        async Task<IEnumerable<Event>> IEventService.Get(FilterEvent filter)
        {
            var query = _context.Events.AsQueryable();
            var courseQuery = _context.Courses.AsQueryable();
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

                    courseQuery = courseQuery.Where(e =>
                    !(e.StartDate.Date > endDate && e.EndDate.Date < fromDate));
                }
                else
                {
                    if (filter.FromDate.HasValue)
                    {
                        query = query.Where(e => e.EndTime > fromDate);
                        courseQuery = courseQuery.Where(e => e.StartDate >= fromDate);
                    }
                    if (filter.ToDate.HasValue)
                    {
                        query = query.Where(e => e.StartTime <= endDate);
                    }
                }
            }

            var listEvents = await query.ToListAsync();
            var listCourses = await courseQuery.ToListAsync();
            var finalEvents = new List<Event>();

            listCourses.ForEach(c =>
            {
                var startDateTemp = c.StartDate > fromDate ? c.StartDate : fromDate;
                var endDateTemp = c.EndDate < endDate ? c.EndDate : endDate;
                var timeSpan = c.EndTime - c.StartTime;

                //Tìm ngày hợp lệ gần nhất
                foreach (var day in c.DayOfWeeks.Split(";").Select(d => (DayOfWeek)int.Parse(d)))
                {
                    var nextDate = Utils.GetNextWeekday(startDateTemp, day);
                    while (nextDate < endDateTemp)
                    {
                        var startTime = nextDate.Date.AddSeconds(c.StartTime);
                        finalEvents.Add(new Event
                        {
                            Id = Guid.NewGuid(),
                            Title = c.Title,
                            Description = c.Description,
                            CourseId = c.Id,
                            StartTime = startTime,
                            EndTime = startTime.AddSeconds(timeSpan),
                            NotiBeforeTime = c.NotiBeforeTime,
                            ColorCode = c.ColorCode,
                            UserId = c.UserId,
                        });
                        nextDate = nextDate.AddDays(7);
                    }
                }
            });

            listEvents.ForEach(e =>
            {
                if (e.RecurringInterval.HasValue && duration != 0 && e.RecurringStart.HasValue)
                {
                    var sameEvents = new List<Event>();
                    var removedEvents = e.GetExceptDates().ToDictionary(t => t, t => t);
                    var startDateTemp = (DateTime)e.RecurringStart > fromDate ? (DateTime)e.RecurringStart : fromDate;
                    var endDateTemp = (e.RecurringEnd.HasValue && e.RecurringEnd < endDate) ? (DateTime)e.RecurringEnd : endDate;
                    var timeSpan = e.EndTime - e.StartTime;

                    //WEEK recurring
                    if (e.RecurringUnit == RecurringUnit.WEEK && e.RecurringDetails != null)
                    {
                        List<DayOfWeek>? dayOfWeeksRecurrings = e.GetRecurringDetails();
                        foreach (var dayOfWeek in dayOfWeeksRecurrings)
                        {
                            startDateTemp = Utils.GetNextWeekday(startDateTemp, dayOfWeek);

                            var recurringCount = Math.Ceiling((endDateTemp - startDateTemp).TotalDays / (7 * (int)e.RecurringInterval));
                            for (int dI = 0; dI < recurringCount; dI++)
                            {
                                var duplicateE = e.Clone();
                                duplicateE.StartTime = Utils.GetNextWeekday(startDateTemp, dayOfWeek).AddDays(7 * (int)e.RecurringInterval * dI);
                                duplicateE.Id = e.StartTime == duplicateE.StartTime ? e.Id : Guid.NewGuid();
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
                    e.CloneEventId = e.Id;
                    finalEvents.Add(e);
                }
            });


            return finalEvents.Where(e => e.StartTime >= fromDate && e.EndTime <= endDate).OrderBy(e => e.StartTime).ToList();
        }
        Task<Event> IEventService.GetById(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IEventService.Delete(string userId, DeleteEventDto dE, CancellationToken cancellationToken)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (!dE.Id.HasValue && !dE.BaseEventId.HasValue)
                {
                    throw new ApiException("Bad request", 400);
                }
                var eventId = dE.CloneEventId;


                string typeId = "Clone";

                var currentEvent = await _context.Events.FindAsync(eventId);
                if (currentEvent == null || currentEvent.UserId.ToString() != userId)
                {
                    throw new ApiException("Event not exist!", 404);
                }

                //nếu như event không lặp thì xóa luôn
                if (!currentEvent.RecurringStart.HasValue || !currentEvent.RecurringUnit.HasValue || !currentEvent.RecurringInterval.HasValue)
                {
                    _context.Remove(currentEvent);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }

                if (!IsValidEventDate((DateTime)currentEvent.RecurringStart!, dE.StartTime, (RecurringUnit)currentEvent.RecurringUnit, (int)currentEvent.RecurringInterval, currentEvent.RecurringDetails)
                || (currentEvent.RecurringEnd.HasValue && dE.StartTime > currentEvent.RecurringEnd))
                {
                    throw new ApiException("Event not exist!", 404);
                }

                if (dE.Id == dE.CloneEventId)
                {
                    typeId = "Main";
                }

                if (typeId != "Main")
                {
                    if (dE.TargetType == TargetType.THIS)
                    {
                        if (currentEvent.BaseEventId != null)
                        {
                            _context.Events.Remove(currentEvent);
                        }
                        else
                        {
                            var exceptDate = (string.IsNullOrEmpty(currentEvent.ExceptDates) ? "" : ";") + TimeSpan.FromTicks(dE.StartTime.Ticks).TotalSeconds;
                            if (currentEvent.ExceptDates.Contains(exceptDate))
                            {
                                throw new ApiException("Event not exist!", 404);
                            }

                            currentEvent.ExceptDates += exceptDate;
                        }
                        await _context.SaveChangesAsync();
                    }
                    else if (dE.TargetType == TargetType.THIS_AND_FOLLOWING)
                    {
                        currentEvent.RecurringEnd = AddTimeByUnit(currentEvent.RecurringUnit ?? RecurringUnit.DAY, dE.StartTime, -1 * (currentEvent.RecurringInterval ?? 0));
                        await _context.Events.Where(e => e.BaseEventId == eventId && e.StartTime > dE.StartTime).DeleteFromQueryAsync(cancellationToken);
                        await _context.SaveChangesAsync();
                    }
                    else // ALL
                    {
                        await _context.Events.Where(e => e.BaseEventId == eventId || e.Id == eventId).DeleteFromQueryAsync(cancellationToken);
                    }

                    await transaction.CommitAsync();
                    return;
                }


                //Trường hợp xóa đúng tk lưu trong DB
                if (dE.TargetType == TargetType.THIS)
                {
                    var nextDateTime = AddTimeByUnit(currentEvent.RecurringUnit ?? RecurringUnit.DAY, dE.StartTime, (currentEvent.RecurringInterval ?? 0));

                    var existEvent = await _context.Events.Where(e => e.BaseEventId == eventId && nextDateTime == e.StartTime).FirstOrDefaultAsync();

                    if (existEvent == null)
                    {
                        var newEv = currentEvent.Clone();
                        newEv.Id = Guid.NewGuid();
                        newEv.StartTime = nextDateTime;
                        newEv.RecurringStart = nextDateTime;
                        await _context.AddAsync(newEv);
                    }
                    else
                    {
                        existEvent.BaseEventId = null;
                        await _context.Events
                        .Where(e => e.BaseEventId == eventId && nextDateTime == e.StartTime)
                        .UpdateFromQueryAsync(e => new Event { BaseEventId = existEvent.Id }, cancellationToken);
                    }

                    _context.Remove(currentEvent);
                    await _context.BulkSaveChangesAsync();
                }
                else
                {
                    await _context.Events.Where(e => e.BaseEventId == eventId || e.Id == eventId).DeleteFromQueryAsync(cancellationToken);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(UpdateEventDto upE, string userId, CancellationToken cancellationToken = default)
        {
            if (!upE.CloneEventId.HasValue && !upE.Id.HasValue && !upE.BaseEventId.HasValue && (upE.BaseEventId.HasValue || !upE.BeforeStartTime.HasValue))
            {
                throw new ApiException("Bad request", 400);
            }
            var eventId = upE.CloneEventId ?? upE.BaseEventId ?? upE.Id;

            string typeId = upE.CloneEventId.HasValue ? "CloneId" : (upE.BaseEventId.HasValue ? "BaseId" : "Main");
            if (upE.Id == upE.CloneEventId)
            {
                typeId = "Main";
            }

            Event? currentEvent = await _context.Events.FindAsync(eventId);
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
                    if (!upE.BeforeStartTime.HasValue)
                    {
                        throw new ApiException("Please provide start time of this event", 400);
                    }

                    if (upE.StartTime.HasValue && upE.StartTime?.Date != upE.BeforeStartTime?.Date)
                    {
                        throw new ApiException("This target type is not supported", 400);
                    }
                    var listNotUsedEvents = await _context.Events.Where(e => e.BaseEventId == eventId).ToListAsync(cancellationToken);
                    _context.Events.RemoveRange(listNotUsedEvents);
                    if (upE.HasRecurringChanged())
                    {
                        currentEvent.ClearExceptDates();
                    }
                    _mapper.Map(upE, currentEvent);
                    await _context.BulkSaveChangesAsync(cancellationToken);
                }
                else if (upE.TargetType == TargetType.THIS)
                {
                    if (typeId == "CloneId")
                    {
                        if (upE.StartTime.HasValue && upE.StartTime != currentEvent.StartTime && upE.EndTime != currentEvent.EndTime)
                        {
                            var exceptDates = currentEvent.GetExceptDates();
                            var newExceptDate = TimeSpan.FromTicks(upE.BeforeStartTime!.Value.Ticks).TotalSeconds.ToString();
                            if (!exceptDates.Contains(newExceptDate))
                            {
                                exceptDates.Add(newExceptDate);
                                currentEvent.ExceptDates = string.Join(";", exceptDates);
                            }
                        }

                        var newEv = currentEvent.SimpleClone();
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

                    var newEv = currentEvent.SimpleClone();
                    _mapper.Map(upE, newEv);
                    newEv.Id = Guid.NewGuid();
                    newEv.BaseEventId = null;
                    newEv.RecurringStart = newEv.StartTime;

                    await _context.Events.AddAsync(newEv);

                    var events = await _context.Events.Where(e => e.BaseEventId == eventId && e.StartTime >= upE.BeforeStartTime).ToListAsync();

                    _context.Events.RemoveRange(events);
                    var recurringInterval = upE.RecurringInterval ?? currentEvent.RecurringInterval;
                    var recurringUnit = upE.RecurringUnit ?? currentEvent.RecurringUnit;
                    if (recurringInterval.HasValue && recurringUnit.HasValue)
                    {
                        var newRecurringEndTime = upE.BeforeStartTime?.Date.AddTimeByRecurringUnit(-1 * recurringInterval ?? 0, (RecurringUnit)recurringUnit).AddSeconds(-1.0 * (double)recurringInterval);
                        if (newRecurringEndTime <= currentEvent.RecurringStart)
                        {
                            currentEvent.UnsubcribeRecurring();
                        }
                        else
                        {
                            currentEvent.RecurringEnd = newRecurringEndTime;
                        }
                    }

                    if (currentEvent.Id == upE.Id)
                    {
                        _context.Events.RemoveRange(events);
                        _context.Events.Remove(currentEvent);
                    }

                    await _context.BulkSaveChangesAsync(cancellationToken);
                }
                return;
            }

            if (upE.TargetType == TargetType.THIS)
            {
                var newEv = currentEvent.Clone();
                newEv.Id = Guid.NewGuid();
                if (currentEvent.RecurringInterval.HasValue)
                {
                    newEv.RecurringEnd = upE.BeforeStartTime?.Date.AddSeconds(1.0 * (double)currentEvent.RecurringInterval);
                }
                _mapper.Map(upE, currentEvent);
                await _context.Events.AddAsync(newEv);
            }
            else
            {
                var listNotUsedEvents = await _context.Events.Where(e => e.BaseEventId == eventId).ToListAsync(cancellationToken);
                _context.Events.RemoveRange(listNotUsedEvents);
                _mapper.Map(upE, currentEvent);
            }
            await _context.BulkSaveChangesAsync(cancellationToken);
        }

    }
}
