namespace Indabo.Core
{
    using System;

    /// <summary>
    /// The type of the logging entry.
    /// 
    /// Debug = Only for internal debugging purpose.
    /// Info = Non critical, only information for the user.
    /// Warning = Non critical, but its important for the user notice. The System might not work as it should do.
    /// Error = Critical, the user needs to be informed. The System does not working as it should do.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Logging of an debug information.
        /// Only for internal debugging purpose.
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Logging of an debug information.
        /// Only for internal debugging purpose.
        /// </summary>
        Trace = 1,

        /// <summary>
        /// Logging of an information.
        /// Non critical, only information for the user.
        /// e.g. Plugin "SamplePlugin1" started!
        /// </summary>
        Info = 2,

        /// <summary>
        /// Logging of an Warning.
        /// Non critical, but its important for the user notice.
        /// The System might not work as it should do.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Logging of an Error.
        /// Critical, the user needs to be informed.
        /// The System does not working as it should do.
        /// </summary>
        Error = 4
    }

    /// <summary>
    /// The logging entry contains information and status foreach logging object.
    /// </summary>
    public class LogEntry
    {
        private static readonly string[] logTypeStrings = { "DEBU", "TRACE", "INFO", "WARN", "ERRO" };

        private readonly LogType logType;
        private readonly DateTime loggingTime;
        private readonly Type sender = null;
        private readonly string message = null;
        private readonly object attachedObject = null;
        private readonly Exception exception = null;

        /// <summary>
        /// The type of the logging entry
        /// </summary>
        public LogType LogType { get => this.logType; }

        /// <summary>
        /// The time of the logging
        /// </summary>
        public DateTime LoggingTime { get => this.loggingTime; }

        /// <summary>
        /// The sender of the message
        /// e.g. FullName of the method
        /// ((Type)myType).FullName
        /// </summary>
        public Type Sender { get => this.sender; }

        /// <summary>
        /// The message that will be logged
        /// </summary>
        public string Message { get => this.message; }

        /// <summary>
        /// Any object that should be attached to the logging
        /// </summary>
        public object AttachedObject { get => this.attachedObject; }

        /// <summary>
        /// The exception that has occured
        /// </summary>
        public Exception Exception { get => this.exception; }

        /// <summary>
        /// Initializes the logging entry using defined 
        /// <paramref name="logType"/>, <paramref name="sender"/> 
        /// and <paramref name="message"/>.
        /// </summary>
        /// <param name="logType">The type of the logging entry</param>
        /// <param name="sender">
        /// The sender of the message
        /// e.g. FullName of the method
        /// ((Type)myType).FullName
        /// </param>
        /// <param name="message">The message that will be logged</param>
        /// <param name="attachedObject">Any object that should be attached to the logging</param>
        /// <param name="exception">The exception that has occured</param>
        public LogEntry(LogType logType, Type sender, string message, object attachedObject = null, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
            }

            this.logType = logType;
            this.loggingTime = DateTime.Now;
            this.sender = sender;
            this.message = message;
            this.attachedObject = attachedObject;
            this.exception = exception;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string str = string.Format(
                "{0} [{1}] {2} - {3}",                
                this.loggingTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                LogEntry.logTypeStrings[(int)this.logType],
                this.sender?.FullName,
                this.message);

            if (this.attachedObject != null)
                str += $"\n{this.attachedObject.ToString()}";

            if (this.exception != null)
            {
                str += $"\n{this.exception.ToString()}";
            }

            return str;
        }
    }
}
