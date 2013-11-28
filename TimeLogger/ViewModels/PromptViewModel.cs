using System;
using System.Windows.Input;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    public class PromptViewModel : ObservableObject
    {
        private bool _canSnooze;
        public bool CanSnooze
        {
            get { return _canSnooze; }
            set
            {
                if (value.Equals(_canSnooze)) return;
                _canSnooze = value;
                OnPropertyChanged();
            }
        }
    }
}