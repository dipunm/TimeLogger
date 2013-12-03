using TimeLogger.Utils.Core;

namespace TimeLogger.Utils.Domain
{
    public class TimerFactory : ITimerFactory
    {
        private readonly IClock _clock;

        public TimerFactory(IClock clock)
        {
            _clock = clock;
        }

        public ITimer CreateTimer()
        {
            return new Timer(_clock);
        }

        public ITimeTracker CreateTimeTracker()
        {
            return new TimeTracker(_clock);
        }
    }
}