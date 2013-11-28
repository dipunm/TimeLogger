using System;
using System.Windows.Input;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    public class PromptViewModel : ObservableObject
    {
        private ICommand _continueCommand;
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

        public void SetContinueAction(Action<object> continueAction)
        {
            ContinueCommand = new DelegateCommand(continueAction);
        }

        public void SetSnoozeAction(Action<object> snoozeAction)
        {
            SnoozeCommand = new DelegateCommand(snoozeAction);
        }
    }
}