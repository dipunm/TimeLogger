using TimeLogger.Utils.Domain;

namespace TimeLogger.Utils.Core
{
    public class TimeFactory : ITimeFactory
    {
        private readonly IClock _clock;

        public TimeFactory(IClock clock)
        {
            _clock = clock;
        }

        public ITimeTracker CreateTimeTracker()
        {
            return new TimeTracker(_clock);
        }
    }
}