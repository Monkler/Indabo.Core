namespace Indabo.Core
{
    using System;

    /// <summary>
    /// Event Args for generel logging.
    /// </summary>
    public class LogEntryEventArgs : EventArgs
    {
        private LogEntry logEntry;

        /// <summary>
        /// The log entry which causes the event to fire.
        /// </summary>
        public LogEntry LogEntry { get => this.logEntry; }

        /// <summary>
        /// Initializes the event args for the generel logging.
        /// </summary>
        /// <param name="logEntry">The log entry which causes the event to fire.</param>
        public LogEntryEventArgs(LogEntry logEntry)
        {
            this.logEntry = logEntry;
        }
    }
}
