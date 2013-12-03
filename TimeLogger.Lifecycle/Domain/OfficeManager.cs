using System;
using System.Collections.Generic;
using System.Linq;
using TimeLogger.Data.Core;
using TimeLogger.Lifecycle.Core;
using TimeLogger.Utils.Core;

namespace TimeLogger.Lifecycle.Domain
{
    public class OfficeManager : IOfficeManager
    {
        private readonly IClock _clock;
        private readonly ITimer _snoozeAllowanceTimer;
        private readonly IWorkRepository _storage;
        private readonly IUserTracker _userTracker;
        private readonly ITimer _workLogTimer;

        private ITimeLoggingConsumer _consumer;

        private DateTime _startTime;
        private Timings _timings;
        private readonly TimeSpan _timespanZeroThreshold;
        private readonly TimeSpan _minRequiredTimeToLog;

        public OfficeManager(
            ITimerFactory timerFactory, IClock clock,
            IWorkRepository storage, IUserTracker userTracker)
        {
            _workLogTimer = timerFactory.CreateTimer();
            _snoozeAllowanceTimer = timerFactory.CreateTimer();
            _clock = clock;
            _storage = storage;
            _userTracker = userTracker;
            _timespanZeroThreshold = TimeSpan.FromTicks(10000);
            _minRequiredTimeToLog = TimeSpan.FromMinutes(1);

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

        private void RequestWorkLogs(ITimer sender)
        {
            if (_consumer != null)
            {
                StartLoggingTime();
            }
        }

        private void DisableSnooze(ITimer sender)
        {
            _consumer.SetSnoozeEnabled(false);
        }

        private void UserTrackerOnUserReturned(IUserTracker sender)
        {
            if (_workLogTimer.InProgress())
                _workLogTimer.FirePendingEvent();
        }

        private void UserTrackerOnUserLeft(IUserTracker sender)
        {
            if (_workLogTimer.InProgress())
                _workLogTimer.HoldEventFire();
        }

        private TimeSpan GetSleepableDuration()
        {
            TimeSpan timeToLog = GetTimeToLog();
            TimeSpan sleepableTime = _timings.SnoozeLimit + _timings.SleepAmount - timeToLog;
            return sleepableTime;
        }

        private TimeSpan GetTimeToLog(DateTime? targetTime = null)
        {
            DateTime now = !targetTime.HasValue ? _clock.Now() : targetTime.Value;
            int minsLogged = _storage.GetLogsForDate(now.Date)
                                     .Sum(log => log.Minutes);
            TimeSpan timeToLog = now - _startTime - TimeSpan.FromMinutes(minsLogged);
            return timeToLog;
        }

        private void StartLoggingTime()
        {
            _workLogTimer.Reset();
            TimeSpan timeToLog = GetTimeToLog();
            if (timeToLog >= _minRequiredTimeToLog)
            {
                TimeSpan sleepableDuration = GetSleepableDuration();
                if (sleepableDuration > TimeSpan.Zero)
                {
                    _consumer.SetSnoozeEnabled(true);
                    _snoozeAllowanceTimer.Reset();
                    _snoozeAllowanceTimer.Duration = sleepableDuration;
                    _snoozeAllowanceTimer.Start();
                }
                else
                {
                    _consumer.SetSnoozeEnabled(false);
                }

                _consumer.LogTime(this, timeToLog);
            }
            else
            {
                Sleep();
            }
        }

        private void Sleep()
        {
            _workLogTimer.Reset();

            TimeSpan timeToLog = GetTimeToLog();
            if (_timings.SleepAmount > timeToLog)
            {
                _workLogTimer.Duration = _timings.SleepAmount - timeToLog;
                _workLogTimer.Start();
            }
            else
            {
                _workLogTimer.FireAndReset();
            }
        }

        public void ClockIn(ITimeLoggingConsumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            _consumer = consumer;
            _timings = _consumer.GetTimings();
            _startTime = _consumer.GetStartTime();
            Sleep();
        }

        public void ClockOut()
        {
            _workLogTimer.Reset();
            DateTime endTime = _consumer.GetEndTime();
            TimeSpan timeToLog = GetTimeToLog(endTime);
            _consumer.SetSnoozeEnabled(false);
            if (timeToLog > _minRequiredTimeToLog)
            {
                _consumer.LogTime(this, timeToLog);
            }
            _consumer = null;
        }

        public void RemindMeInABit()
        {
            _workLogTimer.Reset();
            TimeSpan sleepableTime = GetSleepableDuration();
            if (sleepableTime <= _timespanZeroThreshold)
            {
                //immediately wakeup.
                _workLogTimer.FireAndReset();
            }
            else
            {
                TimeSpan snoozeDuration = _timings.SnoozeAmount <= sleepableTime
                                             ? _timings.SnoozeAmount
                                             : sleepableTime;

                _workLogTimer.Duration = snoozeDuration;
                _workLogTimer.Start();
            }
        }

        public void SubmitWork(IList<WorkLog> work)
        {
            var date = _clock.Now().Date;
            foreach (WorkLog item in work)
            {
                item.Date = date;
                _storage.AddLog(item);
            }

            if (_consumer != null)
            {
                Sleep();
            }
        }

        public void ForceLoggingTime()
        {
            StartLoggingTime();
        }
    }
}