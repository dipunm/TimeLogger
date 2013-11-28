using System;
using TimeLogger.Core.Utils;
using TimeLogger.Testing.Views;

namespace TimeLogger.Testing
{
    public class TestClock : IClock
    {
        private readonly ClockViewModel _clockViewModel = new ClockViewModel();

        public DateTime Now()
        {
            _clockViewModel.Date = DateTime.Now;
            var window = new ClockWindow();
            window.DataContext = _clockViewModel;
            window.ShowDialog();
            return _clockViewModel.Date;
        }
    }
}