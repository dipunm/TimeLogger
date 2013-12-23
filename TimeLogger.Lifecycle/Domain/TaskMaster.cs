using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TimeLogger.Cache.Core;
using TimeLogger.Utils.Core;
using TimeLogger.Utils.Domain;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Main
{
    class TaskMaster : ITaskMaster
    {
        private State _state;
        private Rules _rules;
        private readonly IAlarmClock _alarmClock;
        private DateTime _startTime;
        private readonly string _sessionKey;
        private readonly ITempStorage _storage;
        private readonly ILogRetriever _ui;
        private IAlarm _alarm;
        private bool _cancelling;
        private readonly object _concurrencyLock = new object();
        private bool _final;
        private bool _finalWaiting;

        public TaskMaster(IAlarmClock alarmClock, ITempStorage storage, ILogRetriever ui)
        {
            _alarmClock = alarmClock;
            _storage = storage;
            _ui = ui;
            _sessionKey = _storage.GenerateSessionKey();
            _state = State.Uninitialised;
            _final = false;
        }

        public void SetRules(Rules rules)
        {
            _rules = rules;
            _ui.LoggingTicketCodes = rules.LoggingTicketCodes;
        }

        public void Begin(DateTime startTime)
        {
            _alarm = _alarmClock.CreateAlarm("workTimer", t => LogAccumulatedTime());
            _startTime = startTime;
            Sleep();
        }

        private void Sleep()
        {
            if (_final)
            {
                _finalWaiting = false;
                return;
            }

            ChangeState(State.Sleeping);
            _alarm.Reset();
            var timeToLog = GetDurationToLog(_alarmClock.Now());
            var sleepAmount = _rules.SleepDuration - timeToLog;
            _alarm.Duration = sleepAmount;
            _alarm.Start();
        }

        private TimeSpan GetDurationToLog(DateTime targetTime)
        {
            var mins = _storage
                .GetAllLogs(_sessionKey)
                .Sum(l => l.Minutes);

            return targetTime - (_startTime + TimeSpan.FromMinutes(mins));
        }

        public void LogAccumulatedTime()
        {
            LogAccumulatedTime(_alarmClock.Now(), false);
        }

        private void LogAccumulatedTime(DateTime targetTime, bool skipPleasantries)
        {
            TimeSpan durationToLog;
            bool askUser = false;
            lock (_concurrencyLock)
            {
                if (_state == State.Active)
                {
                    _cancelling = true;
                    var duration = GetDurationToLog(targetTime);
                    if (duration > TimeSpan.Zero)
                    {
                        if (_ui.TryUpdateRunningThread(duration))
                        {
                            _cancelling = false;
                            return;
                        }
                    }
                }

                ChangeState(State.Active);
            
                _alarm.Reset();
                durationToLog = GetDurationToLog(targetTime);
                if (durationToLog > TimeSpan.Zero)
                {
                    askUser = true;
                }
            }
            if (askUser)
            {
                LoggingResponse response = _ui.GetWorkLogs(durationToLog, skipPleasantries);
                lock (_concurrencyLock)
                {
                    if (response.Type == LoggingResponseType.Snooze)
                    {
                        if (!_cancelling)
                        {
                            Snooze();
                        }
                        else
                        {
                            _cancelling = false;
                        }
                    }
                    else
                    {
                        foreach (var log in response.Logs)
                        {
                            log.Date = targetTime.Date;
                        }

                        SendToStorage(response.Logs);
                        Sleep();
                    }
                }
            }
            else
            {
                lock (_concurrencyLock)
                {
                    Sleep();
                }
            }
        }

        private void SendToStorage(IEnumerable<WorkLog> logs)
        {
            _storage.AddRange(logs, _sessionKey);
        }

        private void Snooze()
        {
            ChangeState(State.Sleeping);
            _alarm.Reset();
            var snoozeableAmount = _rules.SleepDuration + _rules.MaximumSnoozeDuration;
            var timeToLog = GetDurationToLog(_alarmClock.Now());
            snoozeableAmount -= timeToLog;
            _alarm.Duration = 
                snoozeableAmount > _rules.SnoozeDuration ? 
                _rules.SnoozeDuration : 
                snoozeableAmount;
            _alarm.Start();
        }

        public void Finalise(DateTime endTime)
        {
            _alarmClock.DestroyAlarm(_alarm);
            _final = true;
            _finalWaiting = true;
            LogAccumulatedTime(endTime, true);
            while (_finalWaiting)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
            ChangeState(State.Uninitialised);
        }

        public event StateChangedEventHandler StateChanged;
        private void ChangeState(State state)
        {

            _state = state;
            if (StateChanged != null)
                StateChanged.Invoke(this, new StateChangedEventArgs(_state));
        }
    }
}