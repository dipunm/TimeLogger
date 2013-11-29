using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimeLogger.Core.OfficeManager;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    class TaskTrayViewModel : ObservableObject
    {
        private readonly Application _app;
        private readonly IOfficeManager _officeManager;
        private readonly ITimeLoggingConsumer _consumer;

        public TaskTrayViewModel(Application app, IOfficeManager officeManager, ITimeLoggingConsumer consumer)
        {
            _app = app;
            _officeManager = officeManager;
            _consumer = consumer;
            MenuItems = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Header = "Clock In",
                        Command = new DelegateCommand(() => _officeManager.ClockIn(_consumer))
                    },
                    new MenuItem()
                    {
                        Header = "Clock Out",
                        Command = new DelegateCommand(_officeManager.ClockOut)
                    },
                    new MenuItem()
                    {
                        Header = "Log Work",
                        Command = new DelegateCommand(_officeManager.ForceLoggingTime)
                    },
                    new MenuItem()
                    {
                        Header = "Quit",
                        Command = new DelegateCommand(_app.Shutdown)
                    }
                };
        }

        public List<MenuItem> MenuItems { get; set; }
    }
}
