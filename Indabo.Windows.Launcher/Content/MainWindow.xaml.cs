namespace Indabo.Windows.Launcher
{
    using System.Reflection;
    using System.Windows;

    using CefSharp.Wpf;

    public partial class MainWindow : Window
    {
        public string WebTitle { get; set; }
        public string WebAddress { get; set; }

        public IWpfWebBrowser WebBrowser { get; set; }

        public MainWindow()
        {
            AssemblyResolver assemblyResolver = new AssemblyResolver(Assembly.GetExecutingAssembly());
            assemblyResolver.Activate();

            Host.Program.Start(new string[] { });

            this.WebAddress = "http://localhost:" + Host.Program.Config.Port;

            this.InitializeComponent();

            this.Height = (SystemParameters.PrimaryScreenHeight * 0.75);
            this.Width = (SystemParameters.PrimaryScreenWidth * 0.66);
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Host.Program.Stop();
        }
    }
}
