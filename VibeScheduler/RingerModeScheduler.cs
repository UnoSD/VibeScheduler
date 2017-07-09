using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;

namespace VibeScheduler
{
    class RingerModeScheduler
    {
        // SetAlarm, SetExactAndAllowWhileIdle and system able to move your alarm:
        // https://www.reddit.com/r/androiddev/comments/59u92p/how_do_i_schedule_exact_offline_alarms_now/
        // https://developer.android.com/reference/android/app/AlarmManager.html
        internal void ScheduleNextUpdate(IReadOnlyCollection<RingerModeSchedule> schedules, Context ctx)
        {
            var now = DateTime.Now;

            var nextSchedule = schedules.Select(schedule => now.NextDateTimeOn(schedule.Days, schedule.From))
                                        .OrderBy(dateTime => dateTime)
                                        .FirstOrDefault();
            
            var intent = new Intent(ctx, typeof(ScheduleReceiver));
            intent.PutExtra("title", "Hello");
            intent.PutExtra("message", "World!");

            var pending = PendingIntent.GetBroadcast(ctx, 0, intent, PendingIntentFlags.UpdateCurrent);

            var manager = ctx.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();

            manager.Cancel(pending);

            var milliseconds = new DateTimeOffset(nextSchedule).ToUnixTimeMilliseconds();

            if (now > nextSchedule)
            {
                pending.Send();
                return;
            }
            
            manager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, milliseconds, pending);
        }
    }
}