using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.MVVM;

namespace TimeLogger.Testing.ViewModels
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
