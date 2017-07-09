using System;

namespace VibeScheduler
{
    public static class DateTimeExtensions
    {
        public static DateTime ToNextDayOfWeek(this DateTime current, DayOfWeek day, bool excludeToday = false) =>
            current.AddDays(current.DaysTo(day, excludeToday));

        public static int DaysTo(this DateTime current, DayOfWeek day, bool excludeToday = false)
        {
            var currentDay = (int)current.DayOfWeek;
            var targetDay = (int)day;
            var totalDaysToAdd = 7 - currentDay + targetDay;

            var daysToAdd = excludeToday && totalDaysToAdd <= 7 ?
                            totalDaysToAdd :
                            totalDaysToAdd % 7;

            return daysToAdd;
        }

        public static DateTime NextDateTimeOn(this DateTime from, WeekDay toDays, TimeSpan toTime)
        {
            var weekDay = from.DayOfWeek.ToWeekDay();

            var hasToday = toDays.HasFlag(weekDay);

            if (hasToday && from.TimeOfDay <= toTime)
                return from.Date + toTime;

            if (!hasToday || toDays != weekDay)
                do
                    weekDay = weekDay.Next<WeekDay>();
                while (!toDays.HasFlag(weekDay));

            return from.ToNextDayOfWeek(weekDay.ToDayOfWeek(), true).Date + toTime;
        }

        public static long ToUnix(this DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        public static DateTime FromUnix(this long milliseconds) => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime.ToLocalTime();

        internal static (DateTime From, DateTime To) ToDateTimeRange(this DateTime fromDateTime, TimeSpan fromTime, TimeSpan toTime)
        {
            var toDateTime = fromDateTime + toTime - fromTime;

            if (toTime <= fromTime)
                toDateTime = toDateTime.AddDays(1);

            return (From: fromDateTime, To: toDateTime);
        }
    }
}