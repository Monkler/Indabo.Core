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

        Dictionary<string, string> wildcards = null;
        int? maxRows = null;

        private List<string> bufferedData;
        private int transferredRowCount = 0;

        public DateTime Timestamp { get => this.timestamp; }

        public int Id { get => this.id; set => this.id = value; }

        public bool IsStoredQuery { get => this.isStoredQuery; set => this.isStoredQuery = value; }

        public string Query { get => this.query; set => this.query = value; }

        public Dictionary<string, string> Wildcards { get => this.wildcards; set => this.wildcards = value; }

        public int? MaxRows { get => this.maxRows; set => this.maxRows = value; }

        public List<string> BufferedData { get => this.bufferedData; set => this.bufferedData = value; }

        public int TransferredRowCount { get => this.transferredRowCount; set => this.transferredRowCount = value; }

        public SQLReaderCommand()
        {
            this.timestamp = DateTime.Now;
            this.bufferedData = new List<string>();
        }

        public override string ToString()
        {
            return $"{this.id}: {this.query}";
        }
    }
}
