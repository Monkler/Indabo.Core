namespace Indabo.Linux
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Indabo.Shared;

    public class Program
    {
        static Program()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            IEnumerable<string> resourceNames = assembly.GetManifestResourceNames();

            foreach (string resourceName in resourceNames)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    AssemblyLoadContext.Default.LoadFromStream(stream);
                }
            }

            AssemblyResolver assemblyResolver = new AssemblyResolver(assembly);
            assemblyResolver.Activate();
        }

        public static void Main(string[] args)
        {
            Host.Program.Start(args);

            while (Console.ReadKey().Key != ConsoleKey.Escape) { }

            Host.Program.Stop();
        }
    }
}
