namespace Indabo.Host
{
    using System;
    using System.Reflection;

    using Indabo.Core;

    public class Program
    {
        private static WebServer webServer;

        public static void Start(string[] args)
        {
            Logging.Info($"Indabo started! - {Assembly.GetExecutingAssembly().FullName}");

            Config config = ConfigFile.Load<Config>("Indabo");

            Program.webServer = new WebServer(config.Port);
            Program.webServer.Start();
        }

        public static void Stop()
        {
            Program.webServer.Stop();

            Logging.Info($"Indabo stopped!");
        }
    }
}
