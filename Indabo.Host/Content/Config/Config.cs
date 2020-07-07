namespace Indabo.Host
{
    using System;

    [Serializable]
    internal class Config
    {
        private const int DEFAULT_PORT = 48623;

        private int port = DEFAULT_PORT;
        private string dbType;

        public int Port { get => this.port; set => this.port = value; }
        public string DBType { get => this.dbType; set => this.dbType = value; }

        public Config() { }
    }
}
