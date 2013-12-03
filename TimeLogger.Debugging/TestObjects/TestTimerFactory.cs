using TimeLogger.Utils.Core;
using TimeLogger.Utils.Domain;

namespace TimeLogger.Debugging.TestObjects
{
    public class TestTimerFactory : ITimerFactory
    {
        private readonly IClock _clock;

        public TestTimerFactory(IClock clock)
        {
            _clock = clock;
        }

        public ITimer CreateTimer()
        {
            return new TestTimer();
        }

        public ITimeTracker CreateTimeTracker()
        {
            return new TimeTracker(_clock);
        }
    }
}
