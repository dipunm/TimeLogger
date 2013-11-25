using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using TimeLogger.Models;
using TimeLogger.Services;
using TimeLogger.ViewModels;
using TimeLogger.Windows;

namespace TimeLogger.Domain
{
    /// <summary>
    /// Mediates the visibility of windows and manages the sleep
    /// functionality
    /// </summary>
    public class Mediator
    {
        private readonly Settings _settingsModel;
        private readonly PromptManager _promptManager;
        private readonly LogTracker _logTracker;
        private readonly IClock _clock;
        private readonly Timer _timer;
        private WelcomeWindow _welcomeWindow;
        private LoggerWindow _loggerWindow;

        public Mediator(Settings settingsModel, PromptManager promptManager, LogTracker logTracker, IClock clock)
        {
            _settingsModel = settingsModel;
            _promptManager = promptManager;
            _logTracker = logTracker;
            _clock = clock;

            _timer = new Timer();
            _timer.Elapsed += (sender, args) => ShowPrompt();
            _timer.AutoReset = false;
        }

        public void Initialise(WelcomeWindow welcomeWindow, LoggerWindow loggerWindow)
        {
            _welcomeWindow = welcomeWindow;
            _loggerWindow = loggerWindow;
        }

        public void ShowWelcomeScreen()
        {
            _welcomeWindow.Dispatcher.Invoke(() => _welcomeWindow.Show(_settingsModel));
        }

        public void Sleep()
        {
            _loggerWindow.Dispatcher.Invoke(() => _loggerWindow.Hide());
            _welcomeWindow.Dispatcher.Invoke(() => _welcomeWindow.Hide());
            
            _timer.Interval = _settingsModel.SleepDuration.TotalMilliseconds;
            _timer.Start();
        }

        public void ShowLogger()
        {
            var minutesToLog = _logTracker.GetMinutesToLog(_clock.Now());
            if (minutesToLog <= 0)
            {
                return;
            }
            _loggerWindow.Dispatcher.Invoke(() => _loggerWindow.Show(minutesToLog, _clock.Now()));
        }

        private void ShowPrompt()
        {
            var timeToLog = TimeSpan.FromMinutes(_logTracker.GetMinutesToLog(_clock.Now()));
            if (timeToLog <= TimeSpan.FromMinutes(1))
            {
                Sleep();
                return;
            }
            var allowanceUsed = timeToLog - _settingsModel.SleepDuration;
            TimeSpan sleepAllowance;
            if (allowanceUsed > TimeSpan.Zero)
            {
                sleepAllowance = _settingsModel.MaxSnoozeLimit - allowanceUsed;
            }
            else
            {
                sleepAllowance = _settingsModel.MaxSnoozeLimit;
            }
            _promptManager.Prompt(sleepAllowance, ShowLogger);
        }

        public void CloseWelcome(DateTime start)
        {
            _logTracker.SetDayStartTime(start);
            _welcomeWindow.Hide();
        }

        public List<string> GetTimeTickets()
        {
            return _settingsModel.TimeLoggingTickets
                .Split(',')
                .Select(s => s.Trim())
                .ToList();
        }
    }
}