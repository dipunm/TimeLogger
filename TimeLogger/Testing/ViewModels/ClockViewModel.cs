using System;
using TimeLogger.MVVM;

namespace TimeLogger.Testing
{
    public class ClockViewModel : ObservableObject
    {
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged();
            }
        }
    }
}