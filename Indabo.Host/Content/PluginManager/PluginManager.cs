using System.Collections.Generic;
using System.IO;

namespace Indabo.Host
{
    internal class PluginManager
    {
        private const string DEFAULT_PLUGIN_FOLDER = "Plugin";

        private List<Plugin> plugins;

        public PluginManager()
        {
            this.plugins = new List<Plugin>();

            if (Directory.Exists(Path.Combine(Program.Config.RootDirectory, DEFAULT_PLUGIN_FOLDER)))
            {
                string[] fileEntries = Directory.GetFiles(Path.Combine(Program.Config.RootDirectory, DEFAULT_PLUGIN_FOLDER));
                foreach (string fileName in fileEntries)
                {
                    this.plugins.Add(new Plugin(fileName));
                }
            }
        }

        public void Start()
        {           
            foreach (Plugin plugin in this.plugins)
            {
                plugin.Start();
            }
        }

        public void Stop()
        {
            foreach (Plugin plugin in this.plugins)
            {
                plugin.Stop();
            }
        }
    }
}
