using System;

namespace TimeLogger.Utils.Core
{
    public interface IAlarm
    {
        TimeSpan Duration { get; set; }
        string Name { get; }
        event TimerElapsedAction Elapsed;

        bool InProgress();
        void Start();
        void Reset();
        void FireAndReset();

        void HoldEventFire();
        void FirePendingEvent();
    }
}