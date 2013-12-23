using System;
using System.Collections.Generic;
using TimeLogger.Utils.Core;

namespace TimeLogger.Utils.Domain
{
    public class AlarmClock : Clock, IAlarmClock
    {
        private readonly IOsTracker _osTracker;
        private readonly Dictionary<string, Tuple<IAlarm, TimerElapsedAction>> _alarms;

        public AlarmClock(IOsTracker osTracker)
        {
            _osTracker = osTracker;
            _alarms = new Dictionary<string, Tuple<IAlarm, TimerElapsedAction>>();
        }

        public IAlarm CreateAlarm(string name, TimerElapsedAction handler)
        {
            if(_alarms.ContainsKey(name))
                throw new InvalidOperationException("Duplicate key. Cannot create new alarm with name: " + name);
            var alarm = new Alarm(this, _osTracker, name);
            alarm.Elapsed += handler;
            _alarms.Add(name, new Tuple<IAlarm, TimerElapsedAction>(alarm, handler));
            return alarm;
        }

        public IAlarm GetAlarm(string name)
        {
            return _alarms[name].Item1;
        }

        public void DestroyAlarm(IAlarm alarm)
        {
            var alarmTuple = _alarms[alarm.Name];
            alarmTuple.Item1.Reset();
            alarmTuple.Item1.Elapsed -= alarmTuple.Item2;
            _alarms.Remove(alarm.Name);
        }
    }
}