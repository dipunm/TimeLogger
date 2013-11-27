using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TimeLogger.Core.Utils;
using TimeLogger.Domain;
using TimeLogger.MVVM;
using TimeLogger.Models;
using TimeLogger.Services;

namespace TimeLogger.ViewModels
{
    public class WelcomeViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly Mediator _mediator;
        private readonly IClock _clock;

        public WelcomeViewModel(Mediator mediator, IClock clock)
        {
            _mediator = mediator;
            _clock = clock;
            Begin = new DelegateCommand(BeginAction);
            StartTime = clock.Now();
        }

        private int _snoozeDurationMins;
        public int SnoozeDurationMins
        {
            get { return _snoozeDurationMins; }
            set
            {
                if (value == _snoozeDurationMins) return;
                _snoozeDurationMins = value;
                OnPropertyChanged();
            }
        }

        private int _sleepDurationMins;
        public int SleepDurationMins
        {
            get { return _sleepDurationMins; }
            set
            {
                if (value == _sleepDurationMins) return;
                _sleepDurationMins = value;
                OnPropertyChanged();
            }
        }

        private int _maxSnoozeDurationMins;
        private DateTime _startTime;
        private Settings _settings;
        private string _timeLoggingTickets;

        public int MaxSnoozeDurationMins
        {
            get { return _maxSnoozeDurationMins; }
            set
            {
                if (value == _maxSnoozeDurationMins) return;
                _maxSnoozeDurationMins = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (value.Equals(_startTime)) return;
                _startTime = value;
                OnPropertyChanged();
            }
        }

        public string TimeLoggingTickets
        {
            get { return _timeLoggingTickets; }
            set
            {
                if (value == _timeLoggingTickets) return;
                _timeLoggingTickets = value;
                OnPropertyChanged();
            }
        }

        public ICommand Begin { get; private set; }
        private void BeginAction()
        {
            _settings.MaxSnoozeLimit = TimeSpan.FromMinutes(MaxSnoozeDurationMins);
            _settings.SleepDuration = TimeSpan.FromMinutes(SleepDurationMins);
            _settings.SnoozeDuration = TimeSpan.FromMinutes(SnoozeDurationMins);
            _settings.TimeLoggingTickets = TimeLoggingTickets;
            
            _mediator.CloseWelcome(StartTime);
            _mediator.Sleep();
        }

        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string columnName] {
            get
            {
                switch (columnName)
                {
                    case "SnoozeDurationMins":
                        if(SnoozeDurationMins < 0)
                            return "Snooze Duration must be positive";
                        if (SnoozeDurationMins >= MaxSnoozeDurationMins)
                            return "Snooze Duration cannot be larger than the Maximum Snooze Duration";
                        return null;
                    case "MaxSnoozeDurationMins":
                        if (MaxSnoozeDurationMins < 0)
                            return "Maximum Snooze Duration must be positive";
                        if(SnoozeDurationMins >= MaxSnoozeDurationMins)
                            return "Maximum Snooze Duration cannot be less than the Maximum Snooze Duration";
                        return null;
                    case "SleepDurationMins":
                        if (SleepDurationMins < 0)
                            return "Sleep Duration must be positive";
                        return null;
                    case "StartTime":
                        if (StartTime.Date != _clock.Now().Date)
                            return "Please enter today's date";
                        if (StartTime.TimeOfDay == TimeSpan.FromSeconds(0))
                            return "Please enter a time";
                        return null;
                    case "TimeLoggingTickets":
                        if (!String.IsNullOrEmpty(TimeLoggingTickets) && Regex.IsMatch(TimeLoggingTickets, @"^([a-zA-Z]+\-\d+\s*,?\s*)+$"))
                            return null;
                        return
                            "Tickets should be in format: AAA-NNN where A is alphabetic characters " +
                            "and N are digits. Separate multiples with commas.";
                    default:
                        return null;
                }
            }
        }

        internal void Bind(Settings settingsModel)
        {
            _settings = settingsModel;
            MaxSnoozeDurationMins = (int)settingsModel.MaxSnoozeLimit.TotalMinutes;
            SleepDurationMins = (int) settingsModel.SleepDuration.TotalMinutes;
            SnoozeDurationMins = (int) settingsModel.SnoozeDuration.TotalMinutes;
            TimeLoggingTickets = settingsModel.TimeLoggingTickets;
        }
    }
}