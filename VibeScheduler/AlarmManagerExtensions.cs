using System;
using Android.App;

namespace VibeScheduler
{
    public static class AlarmManagerExtensions
    {
        public static void ScheduleIntentExactAllowIdle(this AlarmManager manager, DateTime dateTime, PendingIntent intent) =>
            manager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, dateTime.ToUnix(), intent);

        public static void ScheduleIntent(this AlarmManager manager, DateTime dateTime, PendingIntent intent) =>
            manager.SetAlarmClock(new AlarmManager.AlarmClockInfo(dateTime.ToUnix(), intent), intent);
    }
}