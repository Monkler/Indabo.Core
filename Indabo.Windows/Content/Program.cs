namespace Indabo.Windows
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class Program
    {
#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
#endif

        public static void Main(string[] args)
        {
#if DEBUG
            AllocConsole();
#endif

            AssemblyResolver assemblyResolver = new AssemblyResolver(Assembly.GetExecutingAssembly());
            assemblyResolver.Activate();

            void StartAction()
            {
                Host.Program.Start(args);

                while (Console.ReadKey().Key != ConsoleKey.Escape) { }

                Host.Program.Stop();
            }
            StartAction();
        }
    }
}
