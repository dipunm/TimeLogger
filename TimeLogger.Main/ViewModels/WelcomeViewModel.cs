using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MVVM.Extensions;
using TimeLogger.Utils.Core;

namespace TimeLogger.Wpf.ViewModels
{
    public class WelcomeViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly IClock _clock;
        private Action _dataConfirmed;

        private int _maxSnoozeDurationMins;
        private int _sleepDurationMins;
        private int _snoozeDurationMins;
        private DateTime _startTime;
        private string _timeLoggingTickets;

        public WelcomeViewModel(IClock clock)
        {
            _clock = clock;
            Begin = new DelegateCommand(BeginAction);
            StartTime = clock.Now();
            SnoozeDurationMins = 2;
            SleepDurationMins = 60;
            MaxSnoozeDurationMins = 10;
            TimeLoggingTickets = "AD-13";
        }

        public int SnoozeDurationMins
        {
            get { return _snoozeDurationMins; }
            set
            {
                if (value == _snoozeDurationMins) return;
                _snoozeDurationMins = value;
                OnPropertyChanged("SnoozeDurationMins");
            }
        }

        public int SleepDurationMins
        {
            get { return _sleepDurationMins; }
            set
            {
                if (value == _sleepDurationMins) return;
                _sleepDurationMins = value;
                OnPropertyChanged("SleepDurationMins");
            }
        }

        public int MaxSnoozeDurationMins
        {
            get { return _maxSnoozeDurationMins; }
            set
            {
                if (value == _maxSnoozeDurationMins) return;
                _maxSnoozeDurationMins = value;
                OnPropertyChanged("MaxSnoozeDurationMins");
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (value.Equals(_startTime)) return;
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        public string TimeLoggingTickets
        {
            get { return _timeLoggingTickets; }
            set
            {
                if (value == _timeLoggingTickets) return;
                _timeLoggingTickets = value;
                OnPropertyChanged("TimeLoggingTickets");
            }
        }

        public ICommand Begin { get; private set; }

        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "SnoozeDurationMins":
                        if (SnoozeDurationMins < 0)
                            return "Snooze Duration must be positive";
                        if (SnoozeDurationMins >= MaxSnoozeDurationMins)
                            return "Snooze Duration cannot be larger than the Maximum Snooze Duration";
                        return null;
                    case "MaxSnoozeDurationMins":
                        if (MaxSnoozeDurationMins < 0)
                            return "Maximum Snooze Duration must be positive";
                        if (SnoozeDurationMins >= MaxSnoozeDurationMins)
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
                        if (!String.IsNullOrEmpty(TimeLoggingTickets) &&
                            Regex.IsMatch(TimeLoggingTickets, @"^([a-zA-Z]+\-\d+\s*,?\s*)+$"))
                            return null;
                        return
                            "Tickets should be in format: AAA-NNN where A is alphabetic characters " +
                            "and N are digits. Separate multiples with commas.";
                    default:
                        return null;
                }
            }
        }

        private void BeginAction()
        {
            if (IsValid() && _dataConfirmed != null)
                _dataConfirmed.Invoke();
        }

        public void OnConfirm(Action action)
        {
            _dataConfirmed = action;
        }

        private bool IsValid()
        {
            return
                String.IsNullOrEmpty(((IDataErrorInfo) this)["SnoozeDurationMins"]) &&
                String.IsNullOrEmpty(((IDataErrorInfo) this)["MaxSnoozeDurationMins"]) &&
                String.IsNullOrEmpty(((IDataErrorInfo) this)["SleepDurationMins"]) &&
                String.IsNullOrEmpty(((IDataErrorInfo) this)["StartTime"]) &&
                String.IsNullOrEmpty(((IDataErrorInfo) this)["TimeLoggingTickets"]);
        }
    }
}