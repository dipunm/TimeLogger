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

        public IAlarm CreateTimer()
        {
            return new TestAlarmClock();
        }

        public ITimeTracker CreateTimeTracker()
        {
            return new TimeTracker(_clock);
        }
    }
}
