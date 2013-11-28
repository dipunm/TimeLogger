using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TimeLogger.Core.Data;
using TimeLogger.Core.OfficeManager;
using TimeLogger.Core.Utils;
using TimeLogger.ViewModels;

namespace TimeLogger.Domain.UI
{
    public class UIConsumer : ITimeLoggingConsumer
    {
        private readonly IClock _clock;
        private readonly DialogController<LoggerViewModel> _loggerView;
        private readonly DialogController<PromptViewModel> _promptView;
        private readonly DialogController<WelcomeViewModel> _welcomeView;
        private DateTime? _startTime;
        private List<string> _timeLoggingTicketCodes;
        private Timings _timings;

        public UIConsumer(IClock clock,
                          DialogController<PromptViewModel> promptView,
                          DialogController<WelcomeViewModel> welcomeView,
                          DialogController<LoggerViewModel> loggerView
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
                ITimeTracker session = officeManager.CreateTrackingSession();
                session.Start();
                result = _promptView.ShowDialog();
                elapsedTime = session.Stop();
            }

            if (result == null || result == false)
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
            _promptView.ViewModel.CanSnooze = enabled;
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
                result = _welcomeView.ShowDialog();
            }

            WelcomeViewModel viewModel = _welcomeView.ViewModel;
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
            ITimeTracker session = officeManager.CreateTrackingSession();
            session.Start();
            _loggerView.ViewModel.SetCompleteAction(work =>
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
            _loggerView.ViewModel.Reset((int)Math.Ceiling(timeToLog.TotalMinutes));

            bool? result = null;
            while (result != true)
            {
                result = _loggerView.ShowDialog();
            }
        }
    }
}