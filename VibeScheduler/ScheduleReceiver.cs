using Android.Content;
using Android.Media;

namespace VibeScheduler
{
    [BroadcastReceiver]
    public class ScheduleReceiver : BroadcastReceiver
    {
        readonly RingerModeScheduler _scheduler;
        readonly RingerModeScheduleRepository _repository;

        public ScheduleReceiver()
        {
            _scheduler = new RingerModeScheduler();
            _repository = new RingerModeScheduleRepository();
        }

        // We need to cancel schedules if deleted from the app.
        public override async void OnReceive(Context context, Intent intent)
        {
            var mode = (RingerMode)intent.GetIntExtra("RingerMode", 0);
            var to = intent.GetLongExtra("To", 0);

            var audioManager = (AudioManager)context.GetSystemService(Context.AudioService);

            audioManager.RingerMode = mode;

            if(to != 0)
                _scheduler.ScheduleNextUpdateWithoutEndTime(to.FromUnix(), RingerMode.Normal, context);
            else
            {
                var schedules = await _repository.GetSchedulesAsync();

                _scheduler.ScheduleNextUpdate(context, schedules);
            }
        }
    }
}