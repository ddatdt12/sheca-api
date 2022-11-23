using Sheca.Error;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Sheca.Common.Enum;

namespace Sheca.Extensions
{
    public static class HeplerExtensions
    {
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return System.Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }
        public static DateTime AddTimeByRecurringUnit(this DateTime date, int value, RecurringUnit recurringUnit) => recurringUnit switch
        {
            RecurringUnit.DAY => date.AddDays(value),
            RecurringUnit.WEEK => date.AddDays(value * 7),
            RecurringUnit.MONTH => date.AddMonths(value),
        };


    }
}
