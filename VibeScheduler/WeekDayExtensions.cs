using System;
using System.Collections.Generic;
using System.Linq;

namespace VibeScheduler
{
    static class WeekDayExtensions
    {
        static readonly IReadOnlyCollection<(DayOfWeek DayOfWeek, WeekDay WeekDay)> Map = 
            new[]
            {
                (DayOfWeek.Sunday, WeekDay.Sunday), 
                (DayOfWeek.Monday, WeekDay.Monday), 
                (DayOfWeek.Tuesday, WeekDay.Tuesday), 
                (DayOfWeek.Wednesday, WeekDay.Wednesday), 
                (DayOfWeek.Thursday, WeekDay.Thursday), 
                (DayOfWeek.Friday, WeekDay.Friday), 
                (DayOfWeek.Saturday, WeekDay.Saturday) 
            };

        internal static WeekDay ToWeekDay(this DayOfWeek day) => Map.Single(m => m.DayOfWeek == day).WeekDay;
        // TODO: Will get an exception if called with multiple flags
        internal static DayOfWeek ToDayOfWeek(this WeekDay day) => Map.Single(m => m.WeekDay == day).DayOfWeek;
    }
}