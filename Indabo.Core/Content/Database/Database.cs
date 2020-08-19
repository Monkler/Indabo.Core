namespace Indabo.Core
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;

    using Microsoft.Data.Sqlite;

    using MySqlConnector;

    public class Database
    {
        public const int DEFAULT_MYSQL__PORT = 3306;
        public const string DEFAULT_MYSQL_SSL_MODE = "Preffered";

        protected static Database instance;

        private DatabaseType databaseType;
        private DbConnection connection;

        protected Database(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == DatabaseType.MySQL)
            {
                this.connection = new MySqlConnection(connectionString);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                this.connection = new SqliteConnection(connectionString);
            }
            else
            {
                Logging.Error($"DatabaseType not supported: {databaseType}");
                return;
            }

            this.databaseType = databaseType;

            this.connection.Open();
        }

        ~Database()
        {
            this.connection.Close();
        }

        /// <summary>
        /// Creates an Database command using the selected <see cref="DatabaseType"/>
        /// and the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <example>
        /// using (DbDataReader reader = command.ExecuteReader())
        /// {
        ///     while (reader.Read())
        ///     {
        ///         string name = reader.GetString(0);
        ///         Console.WriteLine($"Hello {name}!");
        ///     }
        /// }
        /// </example>
        /// <returns>The <see cref="MySqlCommand"/> or <see cref="SqliteCommand"/> depending on the <see cref="DatabaseType"/></returns>
        public DbCommand ExecuteCommand(string sql)
        {
            DbCommand command;
            if (this.databaseType == DatabaseType.MySQL)
            {
                command = new MySqlCommand(sql, this.connection as MySqlConnection);
            }
            else if (this.databaseType == DatabaseType.SQLite)
            {
                command = new SqliteCommand(sql, this.connection as SqliteConnection);
            }
            else
            {
                Logging.Error($"DatabaseType not supported: {this.databaseType}");
                return null;
            }

            return command;
        }

        public static Database Instance
        {
            get
            {
                if (Database.instance == null)
                {
                    Logging.Error("Database is not initalized yet!");
                    throw new Exception("Database is not initalized yet!");
                }

                return Database.instance;
            }
        }
    }
}
