using System;
using Android.App;

namespace VibeScheduler
{
    public static class AlarmManagerExtensions
    {
        public static void ScheduleIntent(this AlarmManager manager, DateTime dateTime, PendingIntent intent) => 
            manager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, dateTime.ToUnix(), intent);
    }
}