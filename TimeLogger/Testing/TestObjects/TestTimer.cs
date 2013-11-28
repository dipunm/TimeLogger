using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Core.Utils;
using TimeLogger.Testing.Views;

namespace TimeLogger.Testing
{
    public class TestTimer : ITimer
    {
        private readonly TimerViewModel _viewModel;
        private readonly TimerWindow _window;
        public TestTimer()
        {
            _viewModel = new TimerViewModel();
            _window = new TimerWindow();
            _window.Show();
            _viewModel.OnElapsed(() => { if (Elapsed != null) Elapsed.Invoke(this); });
        }

        public TimeSpan Duration
        {
            get { return _viewModel.Duration; }
            set { _viewModel.Duration = value; }
        }

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
