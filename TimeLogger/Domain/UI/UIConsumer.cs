using System;
using System.Collections.Generic;
using System.Linq;
using TimeLogger.Core.Data;
using TimeLogger.Core.OfficeManager;
using TimeLogger.Core.Utils;
using TimeLogger.ViewModels;

namespace TimeLogger.Domain.UI
{
    public class UIConsumer : ITimeLoggingConsumer
    {
        private readonly IClock _clock;
        private readonly WindowViewModelController<LoggerViewModel> _loggerView;
        private readonly WindowViewModelController<PromptViewModel> _promptView;
        private readonly WindowViewModelController<WelcomeViewModel> _welcomeView;
        private DateTime _startTime;
        private List<string> _timeLoggingTicketCodes;
        private Timings _timings;

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
                CollectFromWelcomeWindow();

                WelcomeViewModel viewModel = _welcomeView.ViewModel;
                _timings = new Timings
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
                CollectFromWelcomeWindow();
                _startTime = _welcomeView.ViewModel.StartTime;
            }
            return _startTime;
        }

        public void LogTime(IOfficeManager officeManager, TimeSpan timeToLog)
        {
            ITimeTracker session = officeManager.CreateTrackingSession();
            session.Start();
            _promptView.ShowWindow();
            _promptView.ViewModel.SetSnoozeAction(_ => officeManager.RemindMeInABit());
            _promptView.ViewModel.SetContinueAction(_ =>
                {
                    TimeSpan elapsedTime = session.Stop();
                    ShowLogger(officeManager, timeToLog + elapsedTime);
                });
        }

        public void SetSnoozeEnabled(IOfficeManager officeManager, bool enabled)
        {
            _promptView.ViewModel.SetSnoozeAction(enabled
                                                      ? _ =>
                                                          {
                                                              _promptView.Window.Hide();
                                                              officeManager.RemindMeInABit();
                                                          }
                                                      : (Action<object>) null);
        }

        public DateTime GetEndTime()
        {
            return _clock.Now(); // GetEndTimeFromWindow
        }

        private void CollectFromWelcomeWindow()
        {
            while (!_welcomeView.Window.DialogResult.HasValue || _welcomeView.Window.DialogResult == false)
            {
                _welcomeView.ShowDialog();
            }
            _welcomeView.Window.DialogResult = null;

            _timeLoggingTicketCodes = _welcomeView.ViewModel.TimeLoggingTickets
                                                  .Split(',')
                                                  .Select(t => t.Trim())
                                                  .ToList();
        }

        private void ShowLogger(IOfficeManager officeManager, TimeSpan timeToLog)
        {
            ITimeTracker session = officeManager.CreateTrackingSession();
            session.Start();
            _loggerView.Window.Show();
            _loggerView.ViewModel.SetCompleteAction(work =>
                {
                    TimeSpan timeSpent = session.Stop();
                    work.Add(new WorkLog
                        {
                            Comment = "Logging Work",
                            Minutes = (int) Math.Ceiling(timeSpent.TotalMinutes),
                            TicketCodes = _timeLoggingTicketCodes
                        });

                    officeManager.SubmitWork(work);
                });
            _loggerView.ViewModel.Reset((int) Math.Ceiling(timeToLog.TotalMinutes));
        }
    }
}