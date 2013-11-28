using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Core.Utils;
using TimeLogger.Domain.Utils;

namespace TimeLogger.Testing.TestObjects
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
