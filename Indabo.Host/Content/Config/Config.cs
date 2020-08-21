namespace Indabo.Host
{
    using System;
    using System.IO;
    using System.Reflection;

    using Indabo.Core;

    [Serializable]
    internal class Config
    {
        public static readonly string ROOT_DIRECTORY = new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath).Directory.FullName;

        private const int DEFAULT_PORT = 48623;

        private int port = DEFAULT_PORT;
        private bool webServerPublicAccess = false;

        private DatabaseType databaseType = DatabaseType.SQLite;

        // SQLite: Data Source=hello.db
        // mySQL: server={server};user id={userID};password={password};port=3306;database={database};SslMode=Preferred
        private string databaseConnectionString = "Data Source=Indabo.db";

        public int Port { get => this.port; set => this.port = value; }

        public bool WebServerPublicAccess { get => this.webServerPublicAccess; set => this.webServerPublicAccess = value; }

        public DatabaseType DatabaseType { get => this.databaseType; set => this.databaseType = value; }

        public string DatabaseConnectionString { get => this.databaseConnectionString; set => this.databaseConnectionString = value; }

        public Config() { }
    }
}
