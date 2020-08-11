namespace Indabo.Host
{
    using System;
    using System.Reflection;

    using Indabo.Core;

    public class Program
    {
        private static WebServer webServer;
        private static Config config;

        public static Config Config { get => config; set => config = value; }

        public static void Start(string[] args)
        {
            Logging.Info($"Indabo started! - {Assembly.GetExecutingAssembly().FullName}");

            Program.config = ConfigFile.Load<Config>("Indabo");

            try
            {
                Program.webServer = new WebServer(Program.config.Port);
                Program.webServer.Start();
            }
            catch(Exception ex)
            {
                Logging.Error("Error while starting the WebServer: ", ex);
            }

        }

        public static void Stop()
        {
            try
            {
                Program.webServer.Stop();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while stopping the WebServer: ", ex);
            }

            Logging.Info($"Indabo stopped!");
        }
    }
}
