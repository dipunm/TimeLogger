using System.Collections.Generic;
using TimeLogger.Core.Data;

namespace TimeLogger.Core.OfficeManager
{
    public interface IOfficeManager
    {
        void ClockIn(ITimeLoggingConsumer loggingConsumer);
        void ClockOut();
        void RemindMeInABit();
        void SubmitWork(IList<WorkLog> work);
    }
}
