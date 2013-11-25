using System;

namespace TimeLogger.Core
{
    public interface ILogTracker
    {
        void SetDayStartTime(DateTime start);
        int GetMinutesToLog(DateTime targetTime);
    }
}