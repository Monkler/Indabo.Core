namespace Indabo.Host
{
    using System;

    [Serializable]
    internal class Config
    {
        private int port;
        private string dbType;

        public int Port { get => port; set => port = value; }
        public string DBType { get => dbType; set => dbType = value; }

        public Config() { }
    }
}
