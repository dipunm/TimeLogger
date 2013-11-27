using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Core.Utils;

namespace TimeLogger.Domain.Utils
{
    public class Timer : ITimer
    {
        private System.Timers.Timer _realTimer;
        private bool _inProgress;

        public Timer()
        {
            _realTimer = new System.Timers.Timer
                {
                    AutoReset = false
                };
        }

        public event TimerElapsedAction Elapsed;
        public bool InProgress()
        {
            return _inProgress;
        }

        public void Start()
        {
            _realTimer.Start();
            _inProgress = true;
        }

        public void Reset()
        {
            _realTimer.Stop();
            _inProgress = false;
        }

        public void HoldEventFire()
        {
            _realTimer.Stop();
        }

        public void FirePendingEvent()
        {
            if(_realTimer.)
            if (Elapsed != null)
            {
                Elapsed.Invoke(this);
            }
        }

        public TimeSpan Duration { get; set; }
    }
}
