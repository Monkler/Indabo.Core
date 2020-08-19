namespace Indabo.Host
{
    using Indabo.Core;

    internal class DatabaseInternal : Database
    {
        protected DatabaseInternal(DatabaseType databaseType, string connectionString) 
            : base(databaseType, connectionString) { }

        public static void Initalize(DatabaseType databaseType, string connectionString)
        {
            DatabaseInternal.instance = new DatabaseInternal(databaseType, connectionString);
        }
    }
}
