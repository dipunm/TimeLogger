using System;
using TimeLogger.MVVM;

namespace TimeLogger.Testing
{
    public class ClockViewModel : ObservableObject
    {
        private DateTime _date;
        private string _callStack;
        private string _description;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        public string CallStack
        {
            get { return _callStack; }
            set
            {
                if (value == _callStack) return;
                _callStack = value;
                OnPropertyChanged("CallStack");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged("Description");
            }
        }
    }
}