using System;
using System.Timers;
using System.Windows.Input;
using TimeLogger.Domain;
using TimeLogger.MVVM;
using TimeLogger.Models;
using TimeLogger.Properties;

namespace TimeLogger.ViewModels
{
    public class PromptViewModel : ObservableObject
    {
        public void SetContinueAction(Action<object> continueAction)
        {
            ContinueCommand = new DelegateCommand(continueAction);
        }

        public void SetSnoozeAction(Action<object> snoozeAction)
        {
            SnoozeCommand = new DelegateCommand(snoozeAction);
        }

        private ICommand _snoozeCommand;
        public ICommand SnoozeCommand
        {
            get { return _snoozeCommand; }
            private set
            {
                if (Equals(value, _snoozeCommand)) return;
                _snoozeCommand = value;
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
    }
}