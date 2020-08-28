namespace Indabo.Windows.Launcher
{
    using System;
    using System.CodeDom;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using CefSharp.Wpf;
    using Indabo.Core;
    using Indabo.Shared;

    public partial class MainWindow : INotifyPropertyChanged
    {
        private const int LOG_ENTRY_CACHE_SIZE = 1024;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<LogEntry> logEntriesCache;
        private ICollectionView logEntriesView;

        private int errorCount = 0;
        private int warningCount = 0;
        private int infoCount = 0;
        private int traceCount = 0;
        private int debugCount = 0;

        private bool showError = true;
        private bool showWarning = true;
        private bool showInfo = true;
        private bool showTrace = false;
        private bool showDebug = false;

        public int ErrorCount { get => this.errorCount; }

        public int WarningCount { get => this.warningCount; }

        public int InfoCount { get => this.infoCount; }

        public int TraceCount { get => this.traceCount; }

        public int DebugCount { get => this.debugCount; }

        public string WebTitle { get; set; }
        public string WebAddress { get; set; }

        public IWpfWebBrowser WebBrowser { get; set; }

        public ObservableCollection<LogEntry> LogEntriesCache { get => this.logEntriesCache; set => this.logEntriesCache = value; }

        public ICollectionView LogEntriesView { get => this.logEntriesView; set => this.logEntriesView = value; }

        public bool ShowError
        {
            get => this.showError; set
            {
                this.showError = value;
                this.ApplyFilter();
            }
        }

        public bool ShowWarning
        {
            get => this.showWarning;
            set
            {
                this.showWarning = value;
                this.ApplyFilter();
            }
        }

        public bool ShowInfo
        {
            get => this.showInfo;
            set
            {
                this.showInfo = value;
                this.ApplyFilter();
            }
        }

        public bool ShowTrace
        {
            get => this.showTrace;
            set
            {
                this.showTrace = value;
                this.ApplyFilter();
            }
        }

        public bool ShowDebug
        {
            get => this.showDebug;
            set
            {
                this.showDebug = value;
                this.ApplyFilter();
            }
        }        

        static MainWindow() 
        {
            AssemblyResolver assemblyResolver = new AssemblyResolver(Assembly.GetExecutingAssembly());
            assemblyResolver.Activate();
        }

        public MainWindow()
        {
            this.logEntriesCache = new ObservableCollection<LogEntry>();
            CollectionViewSource itemSourceList = new CollectionViewSource() { Source = this.logEntriesCache };
            this.logEntriesView = itemSourceList.View;

            Logging.NewLogEntry += this.OnLoggingNewLogEntry;

            Host.Program.Start(new string[] { });

            this.WebAddress = "http://localhost:" + Host.Program.Config.Port;

            this.InitializeComponent();

            this.Height = (SystemParameters.PrimaryScreenHeight * 0.75);
            this.Width = (SystemParameters.PrimaryScreenWidth * 0.66);

            this.ApplyFilter();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Host.Program.Stop();
        }

        private void OnLoggingNewLogEntry(object sender, LogEntryEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.logEntriesCache.Add(e.LogEntry);

                if (this.logEntriesList != null)
                {
                    if (VisualTreeHelper.GetChild(this.logEntriesList, 0) is Decorator border)
                    {
                        if (border.Child is ScrollViewer scroll) scroll.ScrollToEnd();
                    }
                }

                if (this.logEntriesCache.Count > LOG_ENTRY_CACHE_SIZE)
                {
                    if (this.logEntriesCache[0].LogType == LogType.Info)
                    {
                        this.infoCount--;
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.InfoCount)));
                    }
                    else if (this.logEntriesCache[0].LogType == LogType.Trace)
                    {
                        this.traceCount++;
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.TraceCount)));
                    }
                    else if (this.logEntriesCache[0].LogType == LogType.Warning)
                    {
                        this.warningCount--;
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WarningCount)));
                    }
                    else if (this.logEntriesCache[0].LogType == LogType.Error)
                    {
                        this.errorCount--;
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ErrorCount)));
                    }
                    else if (this.logEntriesCache[0].LogType == LogType.Debug)
                    {
                        this.debugCount--;
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DebugCount)));
                    }

                    this.logEntriesCache.RemoveAt(1);
                }

                if (e.LogEntry.LogType == LogType.Info)
                {
                    this.infoCount++;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.InfoCount)));
                }
                else if (e.LogEntry.LogType == LogType.Trace)
                {
                    this.traceCount++;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.TraceCount)));
                }
                else if (e.LogEntry.LogType == LogType.Warning)
                {
                    this.warningCount++;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WarningCount)));
                }
                else if (e.LogEntry.LogType == LogType.Error)
                {
                    this.errorCount++;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ErrorCount)));
                }
                else if (e.LogEntry.LogType == LogType.Debug)
                {
                    this.debugCount++;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DebugCount)));
                }
            });
        }

        private void OnExportToFileClick(object sender, RoutedEventArgs e)
        {
            Logging.Error("OnExportToFileClick is not implemented yet!");

            /*System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                LogFileWriter.Instance.UserExport(dialog.SelectedPath);
            }*/
        }

        private void OnLogEntriesListCopyToClipboard(object sender, RoutedEventArgs e)
        {
            string clipboardText = "";
            foreach (LogEntry logEntry in this.logEntriesList.SelectedItems)
            {
                clipboardText += logEntry.ToString() + "\n";
            }

            Clipboard.SetText(clipboardText);
        }

        public void ApplyFilter()
        {
            if (this.logEntriesView != null)
            {
                this.logEntriesView.Filter = new Predicate<object>(item =>
                {
                    LogEntry entry = (LogEntry)item;

                    if (this.showInfo && entry.LogType == LogType.Info)
                        return true;

                    if (this.showTrace && entry.LogType == LogType.Trace)
                        return true;

                    if (this.showWarning && entry.LogType == LogType.Warning)
                        return true;

                    if (this.showError && entry.LogType == LogType.Error)
                        return true;

                    if (this.showDebug && entry.LogType == LogType.Debug)
                        return true;

                    return false;
                });
            }
        }
    }
}
