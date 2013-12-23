using System;

namespace TimeLogger.Utils.Core
{
    public interface IAlarmClock : IClock
    {
        IAlarm CreateAlarm(string name, TimerElapsedAction handler);
        IAlarm GetAlarm(string name);
        void DestroyAlarm(IAlarm alarm);
    }

    public delegate void TimerElapsedAction(IAlarm sender);
}