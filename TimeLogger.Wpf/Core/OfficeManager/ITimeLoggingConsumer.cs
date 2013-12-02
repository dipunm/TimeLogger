using System;

namespace TimeLogger.Core.OfficeManager
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