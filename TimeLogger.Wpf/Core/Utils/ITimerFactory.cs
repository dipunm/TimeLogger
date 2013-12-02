using TimeLogger.Core.OfficeManager;

namespace TimeLogger.Core.Utils
{
    public interface ITimerFactory
    {
        ITimer CreateTimer();
        ITimeTracker CreateTimeTracker();
    }
}