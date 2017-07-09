using System;
using System.Linq;
using Android.Content;
using Android.Media;

namespace VibeScheduler
{
    [BroadcastReceiver]
    public class ScheduleReceiver : BroadcastReceiver
    {
        readonly RingerModeScheduleRepository _repository;
        readonly RingerModeScheduler _scheduler;

        public ScheduleReceiver()
        {
            _repository = new RingerModeScheduleRepository();
            _scheduler = new RingerModeScheduler();
        }

        public override async void OnReceive(Context context, Intent intent)
        {
            var schedules = await _repository.GetSchedulesAsync().ConfigureAwait(false);
            
            var schedule = schedules.FirstOrDefault(Match);

            var audioManager = (AudioManager)context.GetSystemService(Context.AudioService);

            if (schedule == null)
            {
                audioManager.RingerMode = RingerMode.Normal;
                return;
            }

            audioManager.RingerMode = schedule.Mode;

            _scheduler.ScheduleNextUpdate(schedules, context);
        }

        static bool Match(RingerModeSchedule arg)
        {
            var now = DateTime.Now;

            if (now.TimeOfDay <= arg.From || now.TimeOfDay >= arg.To)
                return false;

            return arg.Days.HasFlag(now.DayOfWeek.ToWeekDay());
        }
    }
}