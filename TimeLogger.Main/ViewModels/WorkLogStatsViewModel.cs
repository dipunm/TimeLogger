using MVVM.Extensions;

namespace TimeLogger.Wpf.ViewModels
{
    public class WorkLogStatsViewModel : ObservableObject
    {
        private int _numberOfMinutesLogged;
        private int _numberOfMinutesReq;

        public int NumberOfMinutesLeft
        {
            get { return NumberOfMinutesRequired - NumberOfMinutesLogged; }
        }
        public int NumberOfMinutesRequired
        {
            get { return _numberOfMinutesReq; }
            private set
            {
                _numberOfMinutesReq = value;
                OnPropertyChanged("NumberOfMinutesRequired");
                OnPropertyChanged("NumberOfMinutesLeft");
                OnPropertyChanged("PercentageComplete");
            }
        }

        public int NumberOfMinutesLogged
        {
            get { return _numberOfMinutesLogged; }
            set
            {
                _numberOfMinutesLogged = value;
                OnPropertyChanged("NumberOfMinutesLogged");
                OnPropertyChanged("NumberOfMinutesLeft");
                OnPropertyChanged("PercentageComplete");
            }
        }

        public float PercentageComplete
        {
            get { return ((float) NumberOfMinutesLogged)/NumberOfMinutesRequired; }
        }

        public void ResetStats(int minutesToLog)
        {
            NumberOfMinutesLogged = 0;
            NumberOfMinutesRequired = minutesToLog;
        }
        
        public void UpdateDuration(int minutesToLog)
        {
            NumberOfMinutesRequired = minutesToLog;
        }
    }
}