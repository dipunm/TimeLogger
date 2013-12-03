using System;

namespace TimeLogger.Lifecycle.Core
{
    public interface ITimeLoggingConsumer
    {
        Timings GetTimings();
        DateTime GetStartTime();

        void LogTime(IOfficeManager officeManager, TimeSpan timeToLog);
        void SetSnoozeEnabled(bool enabled);

        DateTime GetEndTime();
    }
}