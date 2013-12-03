
namespace TimeLogger.Utils.Core
{
    public interface ITimerFactory
    {
        ITimer CreateTimer();
        ITimeTracker CreateTimeTracker();
    }
}