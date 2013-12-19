using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace TimeLogger.Tests.Unit.Mocks
{
    class MockClock : IMockClock
    {
        private DateTime _time;

        public MockClock()
        {
            TickAccuracy = TimeSpan.FromTicks(1);
        }

        public TimeSpan TickAccuracy { get; private set; }

        private void NotifyTick()
        {
            if (TimeChanged != null)
                TimeChanged.Invoke(Now());
        }

        public void SetTime(DateTime time)
        {
            if(_time > time)
                throw new InvalidOperationException("Only fast forward time. Reversing time would provide an inaccurate test because time does not go backwards");
            
            _time = time;
            NotifyTick();
        }

        public DateTime Now()
        {
            return _time;
        }

        public event Action<DateTime> TimeChanged;
    }
}
