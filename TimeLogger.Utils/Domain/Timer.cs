using System;
using System.Diagnostics;
using TimeLogger.Utils.Core;

namespace TimeLogger.Utils.Domain
{
    public class Timer : ITimer
    {
        private readonly IClock _clock;
        private readonly System.Timers.Timer _realTimer;
        private DateTime? _beganTime;
        private bool _inProgress;

        public Timer(IClock clock)
        {
            _clock = clock;
            _realTimer = new System.Timers.Timer
                {
                    AutoReset = false
                };
            _realTimer.Elapsed += (sender, args) => OnElapsed();
        }

        public event TimerElapsedAction Elapsed;

        public bool InProgress()
        {
            return _inProgress && _beganTime.HasValue;
        }

        public void Start()
        {
            _beganTime = _clock.Now();
            _realTimer.Interval = Duration.TotalMilliseconds;
            _realTimer.Start();
            _inProgress = true;
        }

        public void Reset()
        {
            _beganTime = null;
            _realTimer.Stop();
            _inProgress = false;
        }

        public void FireAndReset()
        {
            this.Reset();
            this.Elapsed.BeginInvoke(this, null, null);
        }

        public void HoldEventFire()
        {
            if (InProgress())
            {
                _realTimer.Stop();
            }
        }

        public void FirePendingEvent()
        {
            if (InProgress())
            {
                _realTimer.Stop(); //incase this hasn't been done yet.
                Debug.Assert(_beganTime != null, "_beganTime != null");
                TimeSpan timeTaken = _clock.Now() - _beganTime.Value;
                if (timeTaken >= Duration)
                {
                    OnElapsed();
                    Reset();
                }
                else
                {
                    TimeSpan timeLeft = Duration - timeTaken;
                    _realTimer.Interval = timeLeft.TotalMilliseconds;
                    _realTimer.Start();
                }
            }
        }

        private void OnElapsed()
        {
            if (Elapsed != null)
            {
                Elapsed.Invoke(this);
            }
        }

        public TimeSpan Duration { get; set; }
    }
}