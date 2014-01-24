using System;
using System.Linq;
using System.Threading;
using TimeLogger.Wpf.Domain.Controllers;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Main
{
    /// <summary>
    /// Should not be accessed by more than one thread at a time!
    /// </summary>
    public class RuleProvider : IRuleProvider
    {
        private readonly DialogController<WelcomeViewModel> _welcomeDialogController;
        private bool _initialised = false;
        private readonly object _concurrencyLock = new object();
        private bool _exiting;

        public RuleProvider(DialogController<WelcomeViewModel> welcomeDialogController)
        {
            _welcomeDialogController = welcomeDialogController;
            _exiting = false;
        }

        private void LoadUI()
        {
            bool? result;
            do
            {
                result = _welcomeDialogController.ShowDialog();
            } while (!_exiting && (result == null || result.Value == false));
            _initialised = true;
        }

        private T EnforceSingleThread<T>(Func<T> action)
        {
            if (Monitor.TryEnter(_concurrencyLock))
            {
                try
                {
                    return action();
                }
                finally
                {
                    Monitor.Exit(_concurrencyLock);
                }
            }
            else
            {
                throw new ThreadStateException("Cannot execute method because another thread is using this object");
            }
        }

        public Rules FetchTimings()
        {
            return EnforceSingleThread(() =>
                {

                    while (!_initialised)
                    {
                        LoadUI();
                    }
                    return new Rules()
                        {
                            MaximumSnoozeDuration =
                                TimeSpan.FromMinutes(_welcomeDialogController.ViewModel.MaxSnoozeDurationMins),
                            SleepDuration = TimeSpan.FromMinutes(_welcomeDialogController.ViewModel.SleepDurationMins),
                            SnoozeDuration = TimeSpan.FromMinutes(_welcomeDialogController.ViewModel.SnoozeDurationMins),
                            LoggingTicketCodes = _welcomeDialogController.ViewModel.TimeLoggingTickets
                                                    .Split(',')
                                                    .Select(t => t.Trim())
                                                    .ToList()
                        };
                });
        }

        public DateTime GetStartTime()
        {
            return EnforceSingleThread(() =>
                {
                    while (!_initialised)
                    {
                        LoadUI();
                    }
                    return _welcomeDialogController.ViewModel.StartTime;
                });
        }

        public DateTime GetEndTime()
        {
            return EnforceSingleThread(() =>
                {
                    _initialised = false;
                    return DateTime.Now;
                });
        }

        public void Exit()
        {
            _exiting = true;
        }
    }
}