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

        private static PluginManager pluginManager;

        public static Config Config { get => config; set => config = value; }

        public static void Start(string[] args)
        {
            Logging.Info($"Indabo started! - {Assembly.GetExecutingAssembly().FullName}");
            Logging.Info($"Core Version: {Assembly.GetAssembly(typeof(Indabo.Core.Logging)).FullName}");

            try
            {
                Program.config = ConfigFile.Load<Config>("Indabo");
            }
            catch (Exception ex)
            {
                Logging.Error("Error while loading config: ", ex);
                return;
            }

            try
            {
                DatabaseInternal.Initalize(Program.config.DatabaseType, Program.config.DatabaseConnectionString);
            }
            catch (Exception ex)
            {
                Logging.Error("Error while initalizing the Database: ", ex);
                return;
            }

            try
            {
                Program.webServer = new WebServer(Program.config.Port, Program.config.WebServerPublicAccess);
                Program.webServer.Start();
            }
            catch(Exception ex)
            {
                Logging.Error("Error while starting the WebServer: ", ex);
                return;
            }

            try
            {
                Program.sqlReader = new SQLReader();
                Program.sqlReader.Start();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while starting the SQLReader: ", ex);
                return;
            }

            try
            {
                Program.pluginManager = new PluginManager();
                Program.pluginManager.Start();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while starting the PluginManager: ", ex);
                return;
            }
        }

        public static void Stop()
        {
            try
            {
                Program.pluginManager.Stop();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while stopping the PluginManager: ", ex);
            }

            try
            {
                Program.sqlReader.Stop();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while stopping the SQLReader: ", ex);
            }

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
