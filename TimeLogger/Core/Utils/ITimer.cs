using System;

namespace TimeLogger.Core.Utils
{
    public delegate void TimerElapsedAction(ITimer sender);

    public interface ITimer
    {
        TimeSpan Duration { get; set; }
        event TimerElapsedAction Elapsed;

        bool InProgress();
        void Start();
        void Reset();

        void HoldEventFire();
        void FirePendingEvent();
    }
}