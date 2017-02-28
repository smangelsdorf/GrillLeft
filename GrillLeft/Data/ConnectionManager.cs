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

        public ConnectionManager()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(System.IO.Path.Combine(appData, "GrillLeft"));
            var file = System.IO.Path.Combine(appData, "GrillLeft", "db.sqlite3");
            this.connection = new SQLiteConnection($"Data Source={file};Version=3");
            connection.Open();

            WithTransaction(t =>
            {
                ExecuteNonQuery(t, @"
                    CREATE TABLE IF NOT EXISTS options
                    (id INTEGER NOT NULL PRIMARY KEY,
                     name VARCHAR(255) NOT NULL UNIQUE,
                     value VARCHAR(255) NOT NULL)
                ");

                MigrateDatabase(t);
            });
        }

        public T WithTransaction<T>(Func<SQLiteTransaction, T> f)
        {
            lock (connection)
            {
                using (var t = connection.BeginTransaction())
                {
                    var result = f(t);
                    t.Commit();
                    return result;
                }
            }
        }

        public void WithTransaction(Action<SQLiteTransaction> f)
        {
            WithTransaction(t =>
            {
                f(t);
                return this;
            });
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

        private void MigrateOnce(SQLiteTransaction t, int version, String sql)
        {
            string currentVersion;
            using (var cmd = new SQLiteCommand("SELECT value FROM options WHERE name = 'DatabaseVersion'", connection))
            {
                currentVersion = cmd.ExecuteScalar() as string;
            }

            if (currentVersion != null && Int32.Parse(currentVersion) >= version)
            {
                return;
            }

            using (var cmd = new SQLiteCommand(sql, connection))
            {
                System.Console.WriteLine(sql);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO options (name, value) VALUES ('DatabaseVersion', ?)", connection))
            {
                var param = new SQLiteParameter();
                param.Value = version;
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
    }
}