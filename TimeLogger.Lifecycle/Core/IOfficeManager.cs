using System.Collections.Generic;
using TimeLogger.Cache.Core;

namespace TimeLogger.Lifecycle.Core
{
    public interface IOfficeManager
    {
        void ClockIn(ITimeLoggingConsumer loggingConsumer);
        void ClockOut();
        void RemindMeInABit();
        void SubmitWork(IList<WorkLog> work);
        void ForceLoggingTime();
    }
}