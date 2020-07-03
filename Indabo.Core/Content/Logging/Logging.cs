namespace Indabo.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Managing the logging for the Indabo System.
    /// </summary>
    public static class Logging
    {  
        /// <summary>
        /// Occures if a new log entry was added.
        /// </summary>
        public static event EventHandler<LogEntryEventArgs> NewLogEntry;

        private const string LOG_FOLDER = "Logging";
        private const string LOG_FILE_EXTENSION = ".log";

        private static readonly object WRITER_LOCK_OBJECT = new object();
        private static readonly object LOG_LOCK_OBJECT = new object();

        private static Stack<LogEntry> logEntries = new Stack<LogEntry>();

        private static bool isWritingToFile = false;

        /// <summary>
        /// List of all current log entries since program has started.
        /// </summary>
        public static Stack<LogEntry> LogEntries { get => logEntries; }

        /// <summary>
        /// Logging of an <paramref name="logEntry"/>.
        /// </summary>
        /// <param name="logEntry">The logging element that will be logged</param>
        public static void Log(LogEntry logEntry)
        {
            lock (LOG_LOCK_OBJECT)
            {
                Logging.logEntries.Push(logEntry);
            }

            Console.WriteLine(logEntry);

            new Thread(() =>
            {
                Logging.WriteLoggingsToFile();
            }).Start();
        }

        private static void Log(LogType type, string message, Exception exception = null, object attachedObject = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
            }

            MethodBase callingMethod = new StackTrace()?.GetFrame(2)?.GetMethod();

            LogEntry entry = new LogEntry(type, callingMethod?.ReflectedType, message, attachedObject, exception);
            Logging.Log(entry);

            NewLogEntry?.Invoke(entry.Sender, new LogEntryEventArgs(entry));
        }

        /// <summary>
        /// Logging of an debug information.
        /// Only for internal debugging purpose.
        /// </summary>
        /// <param name="message">The message that will be logged</param>
        /// <param name="attachedObject">Any object that should be attached to the logg</param>
        public static void Debug(string message, object attachedObject = null)
        {
            Logging.Log(LogType.Debug, message, null, attachedObject);
        }

        /// <summary>
        /// Logging of an information.
        /// Not needed for the user to read but could be intresting for him. 
        /// e.g. Creating mapping for Channel 'CH_1'...
        /// </summary>
        /// <param name="message">The message that will be logged</param>
        public static void Trace(string message)
        {
            Logging.Log(LogType.Trace, message);
        }

        /// <summary>
        /// Logging of an information.
        /// Non critical, only information for the user.
        /// e.g. Open Loop Model created!
        /// </summary>
        /// <param name="message">The message that will be logged</param>
        public static void Info(string message)
        {
            Logging.Log(LogType.Info, message);
        }

        /// <summary>
        /// Logging of an Warning.
        /// Non critical, but its important for the user notice.
        /// The System might not work as it should do.
        /// </summary>
        /// <param name="message">The message that will be logged</param>
        /// <param name="exception">The exception that has occured</param>
        public static void Warning(string message, Exception exception = null)
        {
            Logging.Log(LogType.Warning, message, exception);
        }

        /// <summary>
        /// Logging of an Error.
        /// Critical, the user needs to be informed.
        /// The System does not working as it should do.
        /// </summary>
        /// <param name="message">The message that will be logged</param>
        /// <param name="exception">The exception that has occured</param>
        public static void Error(string message, Exception exception = null)
        {
            Logging.Log(LogType.Error, message, exception);
        }

        private static void WriteLoggingsToFile()
        {
            try
            {
                if (isWritingToFile == false)
                {
                    lock (Logging.WRITER_LOCK_OBJECT)
                    {
                        isWritingToFile = true;

                        Logging.isWritingToFile = true;

                        while (Logging.logEntries.Count != 0)
                        {
                            string logFolderAbsolutePath = Path.Combine(new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath).Directory.FullName, Logging.LOG_FOLDER);

                            Directory.CreateDirectory(logFolderAbsolutePath);

                            lock (Logging.LOG_LOCK_OBJECT)
                            {
                                LogEntry logEntry = Logging.logEntries.Peek();

                                StreamWriter sw = File.AppendText(Path.Combine(logFolderAbsolutePath, DateTime.Now.ToString("yyyy-MM-dd") + Logging.LOG_FILE_EXTENSION));

                                try
                                {
                                    sw.WriteLine(logEntry.ToString());
                                    Logging.logEntries.Pop();
                                }
                                finally
                                {
                                    sw.Close();
                                }
                            }
                        }

                        Logging.isWritingToFile = false;
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
