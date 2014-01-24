using System.Collections.Generic;
using TimeLogger.Cache.Core;

namespace TimeLogger.Main
{
    public class LoggingResponse
    {
        private LoggingResponse(LoggingResponseType type, ICollection<WorkLog> logs)
        {
            Type = type;
            Logs = logs;
        }

        public static LoggingResponse SnoozeResponse
        {
            get { return new LoggingResponse(LoggingResponseType.Snooze, null); }
        }

        public static LoggingResponse WithLogs(ICollection<WorkLog> logs)
        {
            return new LoggingResponse(LoggingResponseType.Logs, logs);
        }

        public LoggingResponseType Type { get; private set; }
        public ICollection<WorkLog> Logs { get; private set; } 
    }
}