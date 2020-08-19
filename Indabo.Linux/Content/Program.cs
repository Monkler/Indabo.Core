namespace Indabo.Linux
{
    using System;
    using System.Reflection;

    using Indabo.Shared;

    public class Program
    {
        static Program()
        {
            AssemblyResolver assemblyResolver = new AssemblyResolver(Assembly.GetExecutingAssembly());
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
