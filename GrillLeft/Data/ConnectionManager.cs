using GrillLeft.Device;
using GrillLeft.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace GrillLeft.Data
{
    internal class ConnectionManager
    {
        private readonly SQLiteConnection connection;
        private readonly long session;

        public ConnectionManager()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(System.IO.Path.Combine(appData, "GrillLeft"));
            var file = System.IO.Path.Combine(appData, "GrillLeft", "db.sqlite3");
            this.connection = new SQLiteConnection($"Data Source={file};Version=3");
            connection.Open();

            long tempSession = -1;

            WithTransaction(t =>
            {
                ExecuteNonQuery(t, @"
                    CREATE TABLE IF NOT EXISTS options
                    (id INTEGER NOT NULL PRIMARY KEY,
                     name VARCHAR(255) NOT NULL UNIQUE,
                     value VARCHAR(255) NOT NULL)
                ");

                MigrateDatabase(t);
                tempSession = BeginSession(t);
            });

            if (tempSession < 0) throw new Exception("Didn't run query after all");
            this.session = tempSession;
        }

        public void RecordReading(ThermometerState state)
        {
            WithTransaction(t =>
            {
                var sql = "INSERT INTO readings (session_id, channel, time, data) VALUES (?, ?, ?, ?)";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    var p = new object[] { session, (int)state.Channel, ToUnixTime(state.Time), state.Data };
                    foreach (var o in p)
                    {
                        var param = new SQLiteParameter();
                        param.Value = o;

                        cmd.Parameters.Add(param);
                    }

                    cmd.ExecuteNonQuery();
                }
            });
        }

        private long BeginSession(SQLiteTransaction t)
        {
            var time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            ExecuteNonQuery(t, $"INSERT INTO SESSIONS (start_time) VALUES ({ToUnixTime(DateTime.Now)})");
            return connection.LastInsertRowId;
        }

        private void WithTransaction(Action<SQLiteTransaction> f)
        {
            lock (connection)
            {
                using (var t = connection.BeginTransaction())
                {
                    f(t);
                    t.Commit();
                }
            }
        }

        private void ExecuteNonQuery(SQLiteTransaction t, String sql)
        {
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private SQLiteDataReader ExecuteQuery(SQLiteTransaction t, String sql)
        {
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                return cmd.ExecuteReader();
            }
        }

        private T ExecuteScalar<T>(SQLiteTransaction t, String sql) where T : class
        {
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                return cmd.ExecuteScalar() as T;
            }
        }

        private void MigrateOnce(SQLiteTransaction t, int version, String sql)
        {
            var currentVersion = ExecuteScalar<string>(t, "SELECT value FROM options WHERE name = 'DatabaseVersion'");

            if (currentVersion != null && Int32.Parse(currentVersion) >= version)
            {
                return;
            }

            ExecuteNonQuery(t, sql);

            using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO options (name, value) VALUES ('DatabaseVersion', ?)", connection))
            {
                var param = new SQLiteParameter();
                param.Value = version.ToString();
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();
            }
        }

        private void MigrateDatabase(SQLiteTransaction t)
        {
            MigrateOnce(t, 1, @"
                CREATE TABLE sessions
                (id INTEGER NOT NULL PRIMARY KEY,
                 name VARCHAR(255),
                 start_time INTEGER NOT NULL)
            ");

            MigrateOnce(t, 2, @"
                CREATE TABLE readings
                (id INTEGER NOT NULL PRIMARY KEY,
                 session_id INTEGER NOT NULL,
                 channel INTEGER NOT NULL,
                 time INTEGER NOT NULL,
                 data BLOB NOT NULL)
            ");
        }

        private Int32 ToUnixTime(DateTime time)
        {
            return (Int32)(time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}