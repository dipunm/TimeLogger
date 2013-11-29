using System.Collections.Generic;
using TimeLogger.Core.Data;
using TimeLogger.Core.Utils;

namespace TimeLogger.Core.OfficeManager
{
    public interface IOfficeManager
    {
        void ClockIn(ITimeLoggingConsumer loggingConsumer);
        void ClockOut();
        void RemindMeInABit();
        void SubmitWork(IList<WorkLog> work);
        void ForceLoggingTime();

        ITimeTracker CreateTrackingSession();
    }
}