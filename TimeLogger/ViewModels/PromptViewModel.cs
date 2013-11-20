using System;
using System.Timers;
using System.Windows.Input;
using TimeLogger.Domain;
using TimeLogger.MVVM;
using TimeLogger.Models;

namespace TimeLogger.ViewModels
{
    public class PromptViewModel : ObservableObject
    {
        private readonly Settings _settings;
        private readonly Timer _sleepTimer;
        private readonly Timer _disableTimer;
        private TimeSpan _allowance;
        private Action _afterSleep;
        private Action _afterWake;

        public PromptViewModel(Settings settings)
        {
            _sleepTimer = new Timer();
            _sleepTimer.Elapsed += (sender, args) => _afterWake();
            _disableTimer = new Timer();
            _disableTimer.Elapsed += (sender, args) => 
                SleepCommand = new DelegateCommand((Action)null);
            
            _settings = settings;
            SleepCommand = new DelegateCommand(Sleep);
            
        }

        public void AfterSleep(Action action)
        {
            _afterSleep = action;
        }

        public void AfterWake(Action action)
        {
            _afterWake = action;
        }

        public void Reset(TimeSpan sleepAllowance, Action<object> continueAction)
        {
            SleepCommand = new DelegateCommand(Sleep);
            ContinueCommand = new DelegateCommand(continueAction);
            _allowance = sleepAllowance;
        }

        public void StartCountdown()
        {
            _disableTimer.Interval = _settings.MaxSnoozeLimit.TotalMilliseconds;
            _disableTimer.Start();
        }

        private ICommand _sleepCommand;
        public ICommand SleepCommand
        {
            get { return _sleepCommand; }
            private set
            {
                if (Equals(value, _sleepCommand)) return;
                _sleepCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _continueCommand;
        public ICommand ContinueCommand
        {
            get { return _continueCommand; }
            private set
            {
                if (Equals(value, _continueCommand)) return;
                _continueCommand = value;
                OnPropertyChanged();
            }
        }

        private void Sleep()
        {
            var minDuration = (_allowance <= _settings.SleepDuration) ?
                                _allowance :
                                _settings.SleepDuration;

            _sleepTimer.Interval = minDuration.TotalMilliseconds;
            _sleepTimer.Start();
            _allowance -= minDuration;
            _afterSleep.Invoke();
        }
    }
}