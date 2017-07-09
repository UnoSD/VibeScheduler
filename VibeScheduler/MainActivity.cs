using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Widget;
using Android.OS;

namespace VibeScheduler
{
    [Activity(Label = "VibeScheduler", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        readonly RingerModeScheduleRepository _repository;
        readonly RingerModeScheduler _scheduler;

        public MainActivity()
        {
            _repository = new RingerModeScheduleRepository();
            _scheduler = new RingerModeScheduler();
        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var add = FindViewById<Button>(Resource.Id.Add);
            var vibrate = FindViewById<CheckBox>(Resource.Id.Vibrate);
            var from = FindViewById<EditText>(Resource.Id.From);
            var to = FindViewById<EditText>(Resource.Id.To);
            var mon = FindViewById<CheckBox>(Resource.Id.Mon);
            var tue = FindViewById<CheckBox>(Resource.Id.Tue);
            var wed = FindViewById<CheckBox>(Resource.Id.Wed);
            var thu = FindViewById<CheckBox>(Resource.Id.Thu);
            var fri = FindViewById<CheckBox>(Resource.Id.Fri);
            var sat = FindViewById<CheckBox>(Resource.Id.Sat);
            var sun = FindViewById<CheckBox>(Resource.Id.Sun);
            var scheduled = FindViewById<ListView>(Resource.Id.Scheduled);

#if DEBUG
            await _repository.ClearSchedulesAsync();

            var fromSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second + 20);

            await _repository.AddScheduleAsync(new RingerModeSchedule
            {
                From = fromSpan,
                To = fromSpan.Add(new TimeSpan(0, 1, 0)),
                Name = "System generated",
                Mode = RingerMode.Vibrate,
                Days = DateTime.Now.DayOfWeek.ToWeekDay()
            });
#endif

            var schedules = await _repository.GetSchedulesAsync();

            // TODO: This needs to be done at startup instead
            _scheduler.ScheduleNextUpdate(this, schedules);

            UpdateUi(scheduled, this, schedules);

            add.Click += async (_, __) => await AddScheduleAsync(GetModel(vibrate, from, to, mon, tue, wed, thu, fri, sat, sun), scheduled, this);

            scheduled.ItemClick += (_, args) => ConfirmRemoveSchedule(scheduled, args);
        }

        void ConfirmRemoveSchedule(ListView list, AdapterView.ItemClickEventArgs args)
        {
            var item = list.GetItemAtPosition(args.Position).Cast<RingerModeSchedule>();

            var alert = new AlertDialog.Builder(this);

            alert.SetTitle("Confirm delete?");
            alert.SetMessage($"You will delete the schedule: {item}");
            alert.SetNegativeButton("Cancel", (_, __) => { });
            alert.SetPositiveButton("Delete", async (_, __) => await RemoveScheduleAsync(item, list));

            var dialog = alert.Create();

            dialog.Show();
        }

        async Task RemoveScheduleAsync(RingerModeSchedule item, ListView list)
        {
            await _repository.RemoveScheduleAsync(item);

            var schedules = await _repository.GetSchedulesAsync();

            UpdateUi(list, this, schedules);

            Toast.MakeText(this, "Deleted", ToastLength.Short).Show();

            _scheduler.ScheduleNextUpdate(this, schedules);
        }

        static void UpdateUi(ListView list, Context ctx, IEnumerable<RingerModeSchedule> schedules) =>
            list.Adapter =
                new ArrayAdapter<RingerModeSchedule>(ctx, Resource.Layout.ListItem, schedules.ToList());

        async Task AddScheduleAsync(RingerModeSchedule ringerModeSchedule, ListView list, Context context)
        {
            await _repository.AddScheduleAsync(ringerModeSchedule);

            var schedules = await _repository.GetSchedulesAsync();

            UpdateUi(list, context, schedules);

            _scheduler.ScheduleNextUpdate(this, schedules);
        }

        static RingerModeSchedule GetModel(ICheckable vibrate, TextView from, TextView to, ICheckable mon, ICheckable tue, ICheckable wed, ICheckable thu, ICheckable fri, ICheckable sat, ICheckable sun) =>
            new RingerModeSchedule
            {
                Mode = vibrate.Checked ? RingerMode.Vibrate : RingerMode.Silent,
                From = DateTime.Parse(from.Text).TimeOfDay,
                To = DateTime.Parse(to.Text).TimeOfDay,
                Days = GetSelectedWeekDays(mon, tue, wed, thu, fri, sat, sun)
            };

        static WeekDay GetSelectedWeekDays(ICheckable mon, ICheckable tue, ICheckable wed, ICheckable thu, ICheckable fri, ICheckable sat, ICheckable sun)
        {
            var days = mon.Checked ? WeekDay.Monday : 0;
            days = days | (tue.Checked ? WeekDay.Tuesday : 0);
            days = days | (wed.Checked ? WeekDay.Wednesday : 0);
            days = days | (thu.Checked ? WeekDay.Thursday : 0);
            days = days | (fri.Checked ? WeekDay.Friday : 0);
            days = days | (sat.Checked ? WeekDay.Saturday : 0);
            days = days | (sun.Checked ? WeekDay.Sunday : 0);
            return days;
        }
    }
}