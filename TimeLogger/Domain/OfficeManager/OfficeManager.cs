using System;
using System.Collections.Generic;
using System.Linq;
using TimeLogger.Core.Data;
using TimeLogger.Core.OfficeManager;
using TimeLogger.Core.Utils;

namespace TimeLogger.Domain.OfficeManager
{
    public class OfficeManager : IOfficeManager
    {
        private readonly ITimerFactory _timerFactory;
        private readonly IClock _clock;
        private readonly ITimer _snoozeAllowanceTimer;
        private readonly IWorkRepository _storage;
        private readonly IUserTracker _userTracker;
        private readonly ITimer _workLogTimer;

        private ITimeLoggingConsumer _consumer;

        private DateTime _startTime;
        private Timings _timings;

        public OfficeManager(
            ITimerFactory timerFactory, IClock clock,
            IWorkRepository storage, IUserTracker userTracker)
        {
            _workLogTimer = timerFactory.CreateTimer();
            _snoozeAllowanceTimer = timerFactory.CreateTimer();
            _timerFactory = timerFactory;
            _clock = clock;
            _storage = storage;
            _userTracker = userTracker;

            SetupComputer();
            SetupTimers();
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
            if (timeToLog > TimeSpan.Zero)
            {
                _consumer.SetSnoozeEnabled(false);
                _consumer.LogTime(this, timeToLog);
            }
            _consumer = null;
        }

        public void RemindMeInABit()
        {
            _workLogTimer.Reset();
            TimeSpan sleepableTime = GetSleepableDuration();
            if (sleepableTime <= TimeSpan.Zero)
            {
                //immediately wakeup.
                _workLogTimer.Duration = TimeSpan.FromTicks(1);
            }
            else
            {
                TimeSpan sleepDuration = _timings.SnoozeAmount <= sleepableTime
                                             ? _timings.SnoozeAmount
                                             : sleepableTime;

                _workLogTimer.Duration = sleepDuration;
            }
            _workLogTimer.Start();
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

                Sleep();
            }
        }

        public void ForceLoggingTime()
        {
            StartLoggingTime();
        }

        public ITimeTracker CreateTrackingSession()
        {
            return _timerFactory.CreateTimeTracker();
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
            if (_workLogTimer.InProgress())
                _workLogTimer.HoldEventFire();
        }

        private void RequestWorkLogs(ITimer sender)
        {
            StartLoggingTime();
        }

        private void StartLoggingTime()
        {
            if (_consumer != null)
            {
                _workLogTimer.Reset();
                TimeSpan timeToLog = GetTimeToLog();
                if (timeToLog >= TimeSpan.Zero)
                {
                    _consumer.LogTime(this, timeToLog);
                }
                else
                {
                    Sleep();
                }
            }
        }

        private void DisableSnooze(ITimer sender)
        {
            if (_consumer != null)
                _consumer.SetSnoozeEnabled(false);
        }

        private void Sleep()
        {
            _workLogTimer.Reset();

            TimeSpan timeToLog = GetTimeToLog();
            if (timeToLog > TimeSpan.Zero)
            {
                if (_timings.SleepAmount > timeToLog)
                    _workLogTimer.Duration = _timings.SleepAmount - timeToLog;
                else
                    _workLogTimer.Duration = TimeSpan.FromMilliseconds(1);
            }
            else
            {
                TimeSpan timeOverLogged = timeToLog.Negate();
                _workLogTimer.Duration = timeOverLogged + _timings.SleepAmount;
            }

            _workLogTimer.Start();
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
    }
}