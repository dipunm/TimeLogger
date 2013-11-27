using System;
using System.Collections.Generic;
using System.Linq;
using TimeLogger.Core.Data;
using TimeLogger.Core.OfficeManager;
using TimeLogger.Core.Utils;
using TimeLogger.Domain.Data;
using TimeLogger.Domain.UI;
using TimeLogger.Domain.Utils;
using TimeLogger.Services;

namespace TimeLogger.Domain.OfficeManager
{
    public class OfficeManager : IOfficeManager
    {
        private readonly ITimer _workLogTimer;
        private readonly ITimer _snoozeAllowanceTimer;
        private readonly IClock _clock;
        private readonly IWorkRepository _storage;
        private readonly IUserTracker _userTracker;

        private ITimeLoggingConsumer _consumer;
        
        private Timings _timings;
        private DateTime _startTime;

        public OfficeManager(
            ITimerFactory timerFactory, IClock clock,
            IWorkRepository storage, IUserTracker userTracker)
        {
            _workLogTimer = timerFactory.CreateTimer();
            _snoozeAllowanceTimer = timerFactory.CreateTimer();
            _clock = clock;
            _storage = storage;
            _userTracker = userTracker;

            SetupComputer();
            SetupTimers();
        }

        private void SetupComputer()
        {
            _userTracker.UserLeft += UserTrackerOnUserLeft;
            _userTracker.UserReturned += UserTrackerOnUserReturned;
        }

        private void SetupTimers()
        {
            _workLogTimer.Elapsed += RequestWorkLogs;
            _workLogTimer.Reset();
            _snoozeAllowanceTimer.Elapsed += DisableSnooze;
            _snoozeAllowanceTimer.Reset();
        }

        private void UserTrackerOnUserReturned(IUserTracker sender)
        {
            _workLogTimer.FirePendingEvent();
        }

        private void UserTrackerOnUserLeft(IUserTracker sender)
        {
            if(_workLogTimer.InProgress())
                _workLogTimer.HoldEventFire();
        }

        private void RequestWorkLogs(ITimer sender)
        {
            if (_consumer != null)
            {
                var timeToLog = GetTimeToLog();
                _consumer.LogTime(this, timeToLog);
            }
        }

        private void DisableSnooze(ITimer sender)
        {
            if(_consumer != null)
                _consumer.SetSnoozeEnabled(false);
        }

        public void ClockIn(ITimeLoggingConsumer consumer)
        {
            if(consumer == null)
                throw new ArgumentNullException("consumer");

            _consumer = consumer;
            _timings = _consumer.GetTimings();
            _startTime = _consumer.GetStartTime();
            Sleep();
        }

        public void ClockOut()
        {
            _workLogTimer.Reset();
            var endTime = _consumer.GetEndTime();
            var timeToLog = GetTimeToLog(endTime);
            if (timeToLog > TimeSpan.Zero)
            {
                _consumer.LogTime(this, timeToLog);
            }
            _consumer = null;
        }

        public void RemindMeInABit()
        {
            _workLogTimer.Reset();
            var sleepableTime = GetSleepableDuration();
            if (sleepableTime <= TimeSpan.Zero)
            {
                //immediately wakeup.
                _workLogTimer.Duration = TimeSpan.FromTicks(1);
            }
            else
            {
                var sleepDuration = _timings.SnoozeAmount <= sleepableTime ? 
                                        _timings.SnoozeAmount : 
                                        sleepableTime;

                _workLogTimer.Duration = sleepDuration;
                
            }
            _workLogTimer.Start();
        }

        public void SubmitWork(IList<WorkLog> work)
        {
            foreach (var item in work)
            {
                _storage.AddLog(item);
            }

            if (_consumer != null)
            {
                var sleepableDuration = GetSleepableDuration();
                if (sleepableDuration > TimeSpan.Zero)
                {
                    _consumer.SetSnoozeEnabled(true);
                    _snoozeAllowanceTimer.Reset();
                    _snoozeAllowanceTimer.Duration = sleepableDuration;
                    _snoozeAllowanceTimer.Start();
                }

                Sleep();
            }
        }

        private void Sleep()
        {
            _workLogTimer.Reset();

            var timeToLog = GetTimeToLog();
            if (timeToLog > TimeSpan.Zero)
            {
                if (_timings.SleepAmount > timeToLog)
                    _workLogTimer.Duration = _timings.SleepAmount - timeToLog;
                else
                    _workLogTimer.Duration = TimeSpan.FromTicks(1);
            }
            else
            {
                var timeOverLogged = timeToLog.Negate();
                _workLogTimer.Duration = timeOverLogged + _timings.SleepAmount;
            }

            _workLogTimer.Start();
        }

        private TimeSpan GetSleepableDuration()
        {
            var timeToLog = GetTimeToLog();
            var sleepableTime = _timings.SnoozeLimit - timeToLog;
            return sleepableTime;
        }

        private TimeSpan GetTimeToLog(DateTime? targetTime = null)
        {
            var now = !targetTime.HasValue ? _clock.Now() : targetTime.Value;
            var minsLogged = _storage.GetLogsForDate(now.Date)
                                     .Sum(log => log.Minutes);
            var timeToLog = now - _startTime - TimeSpan.FromMinutes(minsLogged);
            return timeToLog;
        }
    }
}