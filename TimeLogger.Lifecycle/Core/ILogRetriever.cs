using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLogger.Main
{
    public enum LoggingResponseType{ Snooze, Logs }
    public interface ILogRetriever
    {
        LoggingResponse GetWorkLogs(TimeSpan durationToLog, bool skipPleasantries);
        bool TryUpdateRunningThread(TimeSpan durationToLog);
        List<string> LoggingTicketCodes { set; }
    }
}
