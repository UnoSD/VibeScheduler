using System;
using Android.Media;
using SQLite;

namespace VibeScheduler
{
    class RingerModeSchedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public RingerMode Mode { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public WeekDay Days { get; set; }

        public override string ToString() => $"{Name}-{From:t}-{To:t}-{Days}-{Mode}";
    }
}