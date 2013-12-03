using System;

namespace TimeLogger.Utils.Core
{
    public delegate void TimerElapsedAction(ITimer sender);

    public interface ITimer
    {
        TimeSpan Duration { get; set; }
        event TimerElapsedAction Elapsed;

        bool InProgress();
        void Start();
        void Reset();
        void FireAndReset();

        void HoldEventFire();
        void FirePendingEvent();
    }
}