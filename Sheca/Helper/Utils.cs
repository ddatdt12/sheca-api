namespace Sheca.Helper
{
    public class Utils
    {
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day, int weekNumbers = 1)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
    }
}
