using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace VibeScheduler
{
    class RingerModeScheduleRepository
    {
        readonly Lazy<Task<SQLiteAsyncConnection>> _connection;

        internal RingerModeScheduleRepository() => 
            _connection = new Lazy<Task<SQLiteAsyncConnection>>(CreateDatabase);

        static async Task<SQLiteAsyncConnection> CreateDatabase()
        {
            var databasePath = GetFilePath();

            var connection = new SQLiteAsyncConnection(databasePath);

            await connection.CreateTableAsync<RingerModeSchedule>()
                            .ConfigureAwait(false);

            return connection;
        }

        internal async Task<IReadOnlyCollection<RingerModeSchedule>> GetSchedulesAsync()
        {
            var connection = await _connection.Value.ConfigureAwait(false);

            return await connection.Table<RingerModeSchedule>().ToListAsync().ConfigureAwait(false);
        }

        internal async Task AddScheduleAsync(RingerModeSchedule schedule)
        {
            var connection = await _connection.Value.ConfigureAwait(false);

            await connection.InsertAsync(schedule).ConfigureAwait(false);
        }

        internal async Task RemoveScheduleAsync(RingerModeSchedule schedule)
        {
            var connection = await _connection.Value.ConfigureAwait(false);
            
            await connection.DeleteAsync(schedule).ConfigureAwait(false);
        }

        static string GetFilePath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(path, "VibeScheduler.sqlite");
            return filePath;
        }
    }
}