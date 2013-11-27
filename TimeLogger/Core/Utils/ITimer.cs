using System;

namespace TimeLogger.Core.Utils
{
    public delegate void TimerElapsedAction(ITimer sender);
    public interface ITimer
    {
        event TimerElapsedAction Elapsed;
        
        bool InProgress();
        void Start();
        void Reset();
        
        void HoldEventFire();
        void FirePendingEvent();

        TimeSpan Duration { get; set; }
    }
}
