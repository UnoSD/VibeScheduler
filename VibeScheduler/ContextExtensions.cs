using Android.App;
using Android.Content;
using Android.Runtime;

namespace VibeScheduler
{
    public static class ContextExtensions
    {
        public static AlarmManager GetAlarmManager(this Context context) =>
            context.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();
    }
}