using System;
using TimeLogger.Utils.Core;

namespace TimeLogger.Utils.Domain
{
    public class TimeTracker : ITimeTracker
    {
        private readonly IClock _clock;
        private DateTime? _start;

        public TimeTracker(IClock clock)
        {
            _clock = clock;
        }

        public void Start()
        {
            if(_start != null)
                throw new InvalidOperationException("Cannot start multiple times.");
            _start = _clock.Now();
        }

        public TimeSpan Stop()
        {
            if (_start == null)
                return TimeSpan.Zero;

            try
            {
                var end = _clock.Now();
                return end - _start.Value;
            }
            finally
            {
                _start = null;
            }
        }
    }
}