namespace Indabo.Host
{
    using System;
    using System.Reflection;

    using Indabo.Core;

    public class Program
    {
        private static Config config;

        private static WebServer webServer;

        private static SQLReader sqlReader;

        internal static Config Config { get => config; set => config = value; }

        public static void Start(string[] args)
        {
            Logging.Info($"Indabo started! - {Assembly.GetExecutingAssembly().FullName}");
            Logging.Info($"Core Version: {Assembly.GetAssembly(typeof(Indabo.Core.Logging)).FullName}");

            Program.config = ConfigFile.Load<Config>("Indabo");

            try
            {
                Program.webServer = new WebServer(Program.config.Port, Program.config.WebServerPublicAccess);
                Program.webServer.Start();
            }
            catch(Exception ex)
            {
                Logging.Error("Error while starting the WebServer: ", ex);
            }

            try
            {
                Program.sqlReader = new SQLReader();
                Program.sqlReader.Start();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while starting the SQLReader: ", ex);
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

            try
            {
                Program.sqlReader.Stop();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while stopping the SQLReader: ", ex);
            }

            Logging.Info($"Indabo stopped!");
        }
    }
}
