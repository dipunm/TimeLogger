using TimeLogger.Utils.Core;

namespace TimeLogger.Utils.Domain
{
    public class TimerFactory : ITimerFactory
    {
        private readonly IClock _clock;
        private readonly IOsTracker _osTracker;

        public TimerFactory(IClock clock, IOsTracker osTracker)
        {
            _clock = clock;
            _osTracker = osTracker;
        }

        public ITimer CreateTimer()
        {
            return new Timer(_clock, _osTracker);
        }

        public ITimeTracker CreateTimeTracker()
        {
            return new TimeTracker(_clock);
        }
    }
}