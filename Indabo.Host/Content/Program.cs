namespace Indabo.Host
{
    using System;
    using System.Reflection;

    using Indabo.Core;

    public class Program
    {
        public static void Main(string[] args)
        {
            Logging.Info($"Indabo started! - {Assembly.GetExecutingAssembly().FullName}");

            Config config = ConfigFile.Load<Config>("Indabo");

            WebServer webServer = new WebServer(config.Port);
            webServer.Start();

            Console.ReadKey();

            webServer.Stop();

            Logging.Info($"Indabo stopped!");

        }
    }
}
