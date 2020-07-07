namespace Indabo.Windows.Launcher
{
    using System.Reflection;
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AssemblyResolver assemblyResolver = new AssemblyResolver(Assembly.GetExecutingAssembly());
            assemblyResolver.Activate();

            Host.Program.Start(new string[] { });

            this.InitializeComponent();

            this.Height = (SystemParameters.PrimaryScreenHeight * 0.75);
            this.Width = (SystemParameters.PrimaryScreenWidth * 0.66);

            this.webBrowser.Navigate("http://localhost:" + Host.Program.Config.Port);
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Host.Program.Stop();
        }
    }
}
