using System.Collections.ObjectModel;
using MVVM.Extensions;

namespace TimeLogger.Debugging.ViewModels
{
    public class ClockLogViewModel : ObservableObject
    {
        public ClockLogViewModel()
        {
            Messages = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Messages { get; set; } 
    }
}
