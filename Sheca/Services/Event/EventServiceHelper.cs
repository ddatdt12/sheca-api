using Sheca.Helper;
using static Sheca.Common.Enum;

namespace Sheca.Services
{
    public partial class EventService
    {

        private bool IsValidEventDate(DateTime originalDate, DateTime date, RecurringUnit unit, int value, string? recurringDetails)
        {
            var days = (date - originalDate).Days;
            if (days < 0 || value <= 0)
            {
                return false;
            }

            switch (unit)
            {
                case RecurringUnit.DAY:
                    return days % value == 0;
                case RecurringUnit.WEEK:
                    if (recurringDetails == null)
                    {
                        return false;
                    }
                    var recurringDetailsList = recurringDetails.Split(";").Select(d => (DayOfWeek)(int.Parse(d)));

                    foreach (var item in recurringDetailsList)
                    {
                        var dayss = (date - Utils.GetNextWeekday(originalDate, item)).Days;
                        if (dayss % (7 * value) == 0)
                        {
                            return true;
                        }
                    }
                    return false;
                case RecurringUnit.MONTH:
                    return date.Date.Day == date.Date.Day;
            }

            return false;
        }

        private DateTime AddTimeByUnit(RecurringUnit unit, DateTime date, int time)
        {
            switch (unit)
            {
                case RecurringUnit.DAY:
                    return date.AddDays(time);
                case RecurringUnit.MONTH:
                    return date.AddMonths(time);
                case RecurringUnit.WEEK:
                    return date.AddDays(time * 7);
                default:
                    break;
            }

            return date;
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

        //private void UpdateStartTime(UpdateEventDto upE, Event originalEvent)
        //{

        //    if (!upE.StartTime.HasValue && upE.RecurringDetails == null & upE?.RecurringDetails?.Count == 0)
        //    {

        //    }
        //    var startTime = upE?.StartTime ?? originalEvent.StartTime;
        //    var endTime = upE?.EndTime ?? originalEvent.EndTime;

        //    DateTime minNextDate = DateTime.MaxValue;
        //    var timeSpan = endTime - startTime;
        //    if (timeSpan.Ticks < 0)
        //    {
        //        throw new ApiException("End Time must greater than start time", 400);
        //    }
        //    foreach (var dOW in e.RecurringDetails)
        //    {
        //        var nextDOW = Utils.GetNextWeekday(e.StartTime, dOW);
        //        if (nextDOW < minNextDate)
        //        {
        //            minNextDate = nextDOW;
        //        }
        //    }
        //    e.StartTime = minNextDate;
        //    e.EndTime = e.StartTime + timeSpan;
        //}

    }
}
