using System;
using TimeLogger.Debugging.ViewModels;
using TimeLogger.Debugging.Views;
using TimeLogger.Utils.Core;

namespace TimeLogger.Debugging.TestObjects
{
    public class TestAlarmClock : IAlarm
    {
        private readonly TimerViewModel _viewModel;
        private readonly TimerWindow _window;
        public TestAlarmClock()
        {
            _viewModel = new TimerViewModel();
            _window = new TimerWindow();
            _window.DataContext = _viewModel;
            _window.Show();
            _viewModel.OnElapsed(() => { if (Elapsed != null) Elapsed.Invoke(this); });
        }

        public TimeSpan Duration
        {
            get { return _viewModel.Duration; }
            set { _viewModel.Duration = value; }
        }

        public string Name { get; private set; }
        public event TimerElapsedAction Elapsed;
        public bool InProgress()
        {
            return _viewModel.InProgress;
        }

        public void Start()
        {
            _viewModel.InProgress = true;
            _viewModel.AddMessage("Started", DateTime.Now);
        }

        public void Reset()
        {
            _viewModel.InProgress = false;
            _viewModel.AddMessage("Reset", DateTime.Now);
        }

        public void FireAndReset()
        {
            _viewModel.InProgress = false;
            _viewModel.AddMessage("FiredAndResetImmediately", DateTime.Now);
        }

        public void HoldEventFire()
        {
            _viewModel.AddMessage("HeldEventFire", DateTime.Now);
        }

        public void FirePendingEvent()
        {
            _viewModel.AddMessage("AttemptingToFire", DateTime.Now);
            if (_viewModel.ShouldFire)
            {
                _viewModel.AddMessage("Firing", DateTime.Now);
                if(Elapsed != null){
                    Elapsed.Invoke(this);
                    _viewModel.AddMessage("Fired", DateTime.Now);
                }
            }
        }
    }
}
