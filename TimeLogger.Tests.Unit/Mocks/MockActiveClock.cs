using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TimeLogger.Utils.Core;

namespace TimeLogger.Tests.Unit.Mocks
{
    /// <summary>
    /// Ticks as fast as real time.
    /// Allows setting of time which will fast-forward
    /// the virtual time.
    /// </summary>
    public class MockActiveClock : IMockClock
    {
        private TimeSpan _difference;
        private DateTime _prevTime;
        private readonly Timer _timer;

        public MockActiveClock()
        {
            TickAccuracy = TimeSpan.FromTicks(1);
            _timer = new Timer()
                {
                    AutoReset = true,
                    Interval = TickAccuracy.TotalMilliseconds
                };
            _timer.Elapsed += (sender, args) => NotifyTick();
            
        }

        public TimeSpan TickAccuracy { get; private set; }

        private void NotifyTick()
        {
            if (TimeChanged != null)
                TimeChanged.Invoke(Now());
        }

        public void SetTime(DateTime time)
        {
            if(_prevTime > time)
                throw new InvalidOperationException("Only fast forward time. Reversing time would provide an inaccurate test because time does not go backwards");
            
            _difference = time - DateTime.Now;
            _prevTime = time;

            NotifyTick();
        }

        public DateTime Now()
        {
            return DateTime.Now + _difference;
        }

        public event Action<DateTime> TimeChanged;
    }
}
