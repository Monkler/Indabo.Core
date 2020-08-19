namespace Indabo.Linux
{
    using System;
    using System.Reflection;

    using Indabo.Shared;

    public class Program
    {
        public static void Main(string[] args)
        {
            AssemblyResolver assemblyResolver = new AssemblyResolver(Assembly.GetExecutingAssembly());
            assemblyResolver.Activate();

            new Action(() =>
            {
                Host.Program.Start(args);

                while (Console.ReadKey().Key != ConsoleKey.Escape) { }

                Host.Program.Stop();
            }).Invoke();
        }
    }
}
