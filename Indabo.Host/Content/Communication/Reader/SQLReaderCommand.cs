namespace Indabo.Host
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    [Serializable]
    internal class SQLReaderCommand
    {
        public static readonly TimeSpan COMMAND_TIMEOUT = new TimeSpan(0, 10, 0);

        private DateTime timestamp;

        private int id;
        private bool isStoredQuery;
        private string query;
        Dictionary<string, string> wildcards;
        int? maxRows = null;

        private DbDataReader dataReader;

        public DateTime Timestamp { get => this.timestamp; set => this.timestamp = value; }

        public int Id { get => this.id; }

        public bool IsStoredQuery { get => this.isStoredQuery; }

        public string Query { get => this.query; }

        public Dictionary<string, string> Wildcards { get => this.wildcards; }

        public int? MaxRows { get => this.maxRows; }

        public DbDataReader DataReader { get => this.dataReader; set => this.dataReader = value; }

        public SQLReaderCommand()
        {
            this.timestamp = DateTime.Now;
        }

        ~SQLReaderCommand()
        {
            if (this.dataReader != null && this.dataReader.IsClosed == false)
            {
                this.dataReader.Close();
            }
        }

        public override string ToString()
        {
            return $"{this.Id}: {this.Query}";
        }
    }
}
