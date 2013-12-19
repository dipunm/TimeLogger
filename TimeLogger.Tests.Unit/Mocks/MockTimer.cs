using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeLogger.Utils.Core;

namespace TimeLogger.Tests.Unit.Mocks
{
    public class MockTimer : ITimer
    {
        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public event TimerElapsedAction Elapsed;

        private readonly IMockClock _clock;
        private bool _inProgress;
        private bool _onHold;
        private DateTime _startTime;
        private TimeSpan _duration;

        public MockTimer(IMockClock clock)
        {
            _clock = clock;
            _clock.TimeChanged += ClockOnTimeChanged;
        }

        private void ClockOnTimeChanged(DateTime dateTime)
        {
            if (_inProgress && !_onHold)
            {
                var timeElapsedSinceTargetTime = dateTime - (_startTime + _duration);
                if (
                    timeElapsedSinceTargetTime <= _clock.TickAccuracy
                    && timeElapsedSinceTargetTime >= TimeSpan.Zero
                    )
                {
                    FireAndReset();
                }
            }
        }

        public bool InProgress()
        {
            return _inProgress;
        }

        public void Start()
        {
            if(_inProgress)
                throw new Exception("Start should never be called whilst in progress!");

            _startTime = _clock.Now();
            _inProgress = true;
        }

        public void Reset()
        {
            _onHold = false;
            _inProgress = false;
        }

        public void FireAndReset()
        {
            if (Elapsed != null)
                Elapsed.Invoke(this);
            Reset();
        }

        public void HoldEventFire()
        {
            if (_inProgress)
            {
                _onHold = true;
            }
        }

        public void FirePendingEvent()
        {
            if (_onHold && _inProgress)
            {
                FireAndReset();
            }
            _onHold = false;
        }
    }
}
