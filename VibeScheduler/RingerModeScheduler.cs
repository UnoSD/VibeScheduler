using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Media;

namespace VibeScheduler
{
    class RingerModeScheduler
    {
        // SetAlarm, SetExactAndAllowWhileIdle and system able to move your alarm:
        // https://www.reddit.com/r/androiddev/comments/59u92p/how_do_i_schedule_exact_offline_alarms_now/
        // https://developer.android.com/reference/android/app/AlarmManager.html
        internal void ScheduleNextUpdate(Context context, IReadOnlyCollection<RingerModeSchedule> schedules)
        {
            var now = DateTime.Now;

            var futureSchedules = schedules.Select(schedule => new
                                            {
                                                Range = now.NextDateTimeOn(schedule.Days, schedule.From)
                                                           .ToDateTimeRange(schedule.From, schedule.To),
                                                Schedule = schedule
                                            })
                                           .OrderBy(schedule => schedule.Range.From)
                                           .ToList();

            if (!futureSchedules.Any())
                return;

            var nextSchedule = futureSchedules.First();

            ScheduleNextUpdate(nextSchedule.Range.From, nextSchedule.Schedule.Mode, context, nextSchedule.Range.To.ToUnix());
        }

        static void ScheduleNextUpdate(DateTime dateTime, RingerMode mode, Context context, long endTimeMilliseconds)
        {
            var intent = GetIntent(context, mode, endTimeMilliseconds);

            var manager = context.GetAlarmManager();

            manager.ScheduleIntent(dateTime, intent);
        }

        internal void ScheduleNextUpdateWithoutEndTime(DateTime dateTime, RingerMode mode, Context context) => 
            ScheduleNextUpdate(dateTime, mode, context, 0);

        static PendingIntent GetIntent(Context context, RingerMode ringerMode, long toMilliseconds)
        {
            var intent = new Intent(context, typeof(ScheduleReceiver));

            intent.PutExtra("RingerMode", (int)ringerMode);
            intent.PutExtra("To", toMilliseconds);

            return PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}