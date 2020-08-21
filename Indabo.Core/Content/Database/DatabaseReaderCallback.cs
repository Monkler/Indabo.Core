namespace Indabo.Core
{
    using System.Data;

    public class DatabaseReaderCallback
    {
        IDataRecord records;
        bool isRequestComplete = false;

        public IDataRecord Records { get => this.records; set => this.records = value; }
        public bool IsRequestComplete { get => this.isRequestComplete; set => this.isRequestComplete = value; }

        public DatabaseReaderCallback(IDataRecord records)
        {
            this.records = records;
            this.isRequestComplete = false;
        }
    }
}
