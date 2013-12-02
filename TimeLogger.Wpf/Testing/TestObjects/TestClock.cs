using System;
using TimeLogger.Core.Utils;
using TimeLogger.Testing.ViewModels;
using TimeLogger.Testing.Views;

namespace TimeLogger.Testing
{
    public class TestClock : IClock
    {
        private readonly ClockViewModel _clockViewModel;
        private readonly ClockLogWindow _logWindow;
        private ClockLogViewModel _clockLogViewModel;

        public TestClock()
        {
            _clockViewModel = new ClockViewModel();
            _logWindow = new ClockLogWindow();
            _clockLogViewModel = new ClockLogViewModel();
            _logWindow.DataContext = _clockLogViewModel;
            _logWindow.Show();
        }

        public DateTime Now()
        {
            _clockViewModel.CallStack = Environment.StackTrace;
            var window = new ClockWindow();
            window.DataContext = _clockViewModel;
            window.ShowDialog();
            _clockLogViewModel.Messages.Add(String.Format("{0} - {1}", _clockViewModel.Description, _clockViewModel.Date));
            return _clockViewModel.Date;
        }
    }
}