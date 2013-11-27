using System;
using TimeLogger.Core.OfficeManager;
using TimeLogger.Core.Utils;
using TimeLogger.Domain.OfficeManager;
using TimeLogger.Services;
using TimeLogger.ViewModels;

namespace TimeLogger.Domain.UI
{
    public class UIConsumer : ITimeLoggingConsumer
    {
        private readonly IClock _clock;
        private readonly WindowViewModelController<PromptViewModel> _promptView;
        private readonly WindowViewModelController<WelcomeViewModel> _welcomeView;
        private readonly WindowViewModelController<LoggerViewModel> _loggerView;
        
        private Timings _timings;
        private DateTime _startTime;

        public UIConsumer(IClock clock, 
            WindowViewModelController<PromptViewModel> promptView,
            WindowViewModelController<WelcomeViewModel> welcomeView, 
            WindowViewModelController<LoggerViewModel> loggerView
            )
        {
            _clock = clock;
            _promptView = promptView;
            _welcomeView = welcomeView;
            _loggerView = loggerView;
        }

        public Timings GetTimings()
        {
            if (_timings == null)
            {
                while (!_welcomeView.Window.DialogResult.HasValue || _welcomeView.Window.DialogResult == false)
                {
                    _welcomeView.ShowDialog();
                }
                
                var viewModel = _welcomeView.ViewModel;
                _timings = new Timings()
                    {
                        SleepAmount = TimeSpan.FromMinutes(viewModel.SleepDurationMins),
                        SnoozeAmount = TimeSpan.FromMinutes(viewModel.SnoozeDurationMins),
                        SnoozeLimit = TimeSpan.FromMinutes(viewModel.MaxSnoozeDurationMins)
                    };
            }
            return _timings;
        }

        public DateTime GetStartTime()
        {
            if (_startTime.Date != _clock.Now().Date)
            {
                while (!_welcomeView.Window.DialogResult.HasValue || _welcomeView.Window.DialogResult == false)
                {
                    _welcomeView.ShowDialog();
                }
                _startTime = _welcomeView.ViewModel.StartTime;
            }
            return _startTime;
        }

        public void LogTime(IOfficeManager officeManager, TimeSpan timeToLog)
        {
            _promptView.ShowWindow();
            _promptView.ViewModel.SetSnoozeAction(_ => officeManager.RemindMeInABit());
            _promptView.ViewModel.SetContinueAction(_ => ShowLogger(officeManager, timeToLog));
        }

        private void ShowLogger(IOfficeManager officeManager,TimeSpan timeToLog)
        {
            _loggerView.Window.Show();
            _loggerView.ViewModel.SetCompleteAction(officeManager.SubmitWork);
            _loggerView.ViewModel.Reset(timeToLog + waitTime);
        }

        public void SetSnoozeEnabled(bool enabled)
        {
            _promptView.ViewModel.SnoozeCommand
        }

        public DateTime GetEndTime()
        {
            throw new NotImplementedException();
        }
    }
}