using System;
using System.Collections.Generic;
using System.Linq;
using TimeLogger.Cache.Core;
using TimeLogger.Lifecycle.Core;
using TimeLogger.Utils.Core;
using TimeLogger.Wpf.Domain.Controllers;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Wpf.Domain
{
    public class UIConsumer : ITimeLoggingConsumer
    {
        private readonly IClock _clock;
        private readonly ITimerFactory _timerFactory;
        private readonly DialogController<LoggerViewModel> _loggerController;
        private readonly DialogController<PromptViewModel> _promptController;
        private readonly DialogController<WelcomeViewModel> _welcomeController;
        private DateTime? _startTime;
        private List<string> _timeLoggingTicketCodes;
        private Timings _timings;

        public UIConsumer(IClock clock, ITimerFactory timerFactory,
                          DialogController<PromptViewModel> promptController,
                          DialogController<WelcomeViewModel> welcomeController,
                          DialogController<LoggerViewModel> loggerController
            )
        {
            _clock = clock;
            _timerFactory = timerFactory;
            _promptController = promptController;
            _welcomeController = welcomeController;
            _loggerController = loggerController;
        }

        public Timings GetTimings()
        {
            if (_timings == null)
            {
                LaunchWelcomeWindow();
            }
            return _timings;
        }

        public DateTime GetStartTime()
        {
            while (_startTime == null)
            {
                LaunchWelcomeWindow();
            }
            return _startTime.Value;
        }

        public void LogTime(IOfficeManager officeManager, TimeSpan timeToLog)
        {
            bool? result;
            TimeSpan elapsedTime;
            if (_startTime == null)
            {
                /*
                 * If _startTime is null, we have reached the end of the day.
                 * It is time to just log and not worry about prompts
                 */
                result = true;
                elapsedTime = TimeSpan.Zero;
            }
            else
            {
                ITimeTracker session = _timerFactory.CreateTimeTracker();
                session.Start();
                result = _promptController.ShowDialog();
                elapsedTime = session.Stop();
            }

            if ((result == null || result == false) && _promptController.ViewModel.CanSnooze)
            {
                officeManager.RemindMeInABit();
            }
            else
            {
                ShowLogger(officeManager, timeToLog + elapsedTime);
            }
        }

        public void SetSnoozeEnabled(bool enabled)
        {
            _promptController.ViewModel.CanSnooze = enabled;
        }

        public DateTime GetEndTime()
        {
            try
            {
                return _clock.Now(); // GetEndTimeFromWindow
            }
            finally
            {
                _startTime = null; //ready for tomorrow!
            }
        }

        private void LaunchWelcomeWindow()
        {
            bool? result = null;
            while (!result.HasValue || result == false)
            {
                result = _welcomeController.ShowDialog();
            }

            WelcomeViewModel viewModel = _welcomeController.ViewModel;
            _startTime = viewModel.StartTime;
            _timings = new Timings
            {
                SleepAmount = TimeSpan.FromMinutes(viewModel.SleepDurationMins),
                SnoozeAmount = TimeSpan.FromMinutes(viewModel.SnoozeDurationMins),
                SnoozeLimit = TimeSpan.FromMinutes(viewModel.MaxSnoozeDurationMins)
            };
            _timeLoggingTicketCodes = viewModel.TimeLoggingTickets
                .Split(',')
                .Select(t => t.Trim())
                .ToList();
        }

        private void ShowLogger(IOfficeManager officeManager, TimeSpan timeToLog)
        {
            ITimeTracker session = _timerFactory.CreateTimeTracker();
            session.Start();
            _loggerController.ViewModel.SetCompleteAction(work =>
                {
                    TimeSpan timeSpent = session.Stop();
                    work.Add(new WorkLog
                        {
                            Comment = "Logging Work",
                            Minutes = (int)Math.Ceiling(timeSpent.TotalMinutes),
                            TicketCodes = _timeLoggingTicketCodes
                        });

                    officeManager.SubmitWork(work);
                });
            _loggerController.ViewModel.Reset((int)Math.Ceiling(timeToLog.TotalMinutes));

            bool? result = null;
            while (result != true)
            {
                result = _loggerController.ShowDialog();
            }
        }
    }
}