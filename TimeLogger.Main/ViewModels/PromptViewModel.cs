using MVVM.Extensions;

namespace TimeLogger.Wpf.ViewModels
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
                OnPropertyChanged("CanSnooze");
            }
        }
    }
}