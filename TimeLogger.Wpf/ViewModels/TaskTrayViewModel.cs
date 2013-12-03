using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MVVM.Extensions;
using TimeLogger.Lifecycle.Core;

namespace TimeLogger.Wpf.ViewModels
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
            MenuItems = new ObservableCollection<MenuItem>()
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

        public ObservableCollection<MenuItem> MenuItems { get; set; }

        public void AddHttpItem(string name, Uri url)
        {
            MenuItems.Add(new MenuItem()
                {
                    Header = name,
                    Command = new DelegateCommand(() => Process.Start(url.AbsoluteUri))
                });
        }
    }
}
