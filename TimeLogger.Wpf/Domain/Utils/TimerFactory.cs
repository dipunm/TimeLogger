using TimeLogger.Core.Utils;

namespace TimeLogger.Domain.Utils
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