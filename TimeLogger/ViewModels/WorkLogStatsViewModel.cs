using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    public class WorkLogStatsViewModel : ObservableObject
    {
        private int _numberOfMinutesLogged;
        private int _numberOfMinutesReq;

        public int NumberOfMinutesRequired
        {
            get { return _numberOfMinutesReq; }
            private set
            {
                _numberOfMinutesReq = value;
                OnPropertyChanged("NumberOfMinutesRequired");
            }
        }

        public int NumberOfMinutesLogged
        {
            get { return _numberOfMinutesLogged; }
            set
            {
                _numberOfMinutesLogged = value;
                OnPropertyChanged("NumberOfMinutesLogged");
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
    }
}