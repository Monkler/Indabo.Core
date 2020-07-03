namespace Indabo.Core
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Managing the config file handling
    /// </summary>
    public class ConfigFile
    {
        /// <summary>
        /// Loads the configuration file from the specified path
        /// and deserializes to an object of type T
        /// </summary>
        /// <typeparam name="T">The type of the config object.</typeparam>
        /// <param name="path">
        /// The relative path to the config file.
        /// e.g. "./Indabo.config" or "./Backend/MyPlugin.config"
        /// </param>
        /// <returns>
        /// The configuration object.
        /// Empty new object if not found.
        /// </returns>
        public static T Load<T>(string path)
        {
            string absolutePath = Path.Combine(new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath).Directory.FullName, path + ".config");

            if (File.Exists(absolutePath))
            {
                try
                {
                    using (FileStream file = File.OpenRead(absolutePath))
                    {
                        using (StreamReader stream = new StreamReader(file))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            T res = (T)serializer.Deserialize(stream, typeof(T));

                            Logging.Info($"Config loaded: {typeof(T).FullName} - '{path}'");

                            return res;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Logging.Error("Could not load config file!", ex);
                }
            }

            Logging.Info($"Config file not found - Creating new: {typeof(T).FullName} - '{path}'");

            T newInstance = (T)Activator.CreateInstance(typeof(T));
            ConfigFile.Save<T>(path, newInstance);
            return newInstance;
        }

        /// <summary>
        /// Saves the given data to the given path.
        /// </summary>
        /// <typeparam name="T">The type of the config object.</typeparam>
        /// <param name="path">
        /// The relative path to the config file.
        /// e.g. "./Indabo.config" or "./Backend/MyPlugin.config"
        /// </param>
        /// <param name="data">The configuration object.</param>
        public static void Save<T>(string path, T data)
        {
            try
            {
                string absolutePath = Path.Combine(new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath).Directory.FullName, path + ".config");

                using (FileStream file = File.OpenWrite(absolutePath))
                {
                    using (StreamWriter stream = new StreamWriter(file))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Formatting.Indented;
                        serializer.Serialize(stream, data);

                        Logging.Info($"Config saved: {typeof(T).FullName} - '{path}'");
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.Error($"Could not save config file: {typeof(T).FullName} - '{path}'", ex);
            }
        }
    }
}
