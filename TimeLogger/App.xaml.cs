using System;
using System.Windows;
using Microsoft.Win32;
using TimeLogger.Domain;
using TimeLogger.Domain.UI;
using TimeLogger.Domain.Utils;
using TimeLogger.Models;
using TimeLogger.Services;
using TimeLogger.ViewModels;
using TimeLogger.Windows;

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // contains settings for the application to run
            var settingsModel = new Settings()
                {
                    MaxSnoozeLimit = TimeSpan.FromMinutes(30),
                    SleepDuration = TimeSpan.FromMinutes(60),
                    SnoozeDuration = TimeSpan.FromMinutes(5),
                    TimeLoggingTickets = "AD-34"
                };
            // manages the showing of the logger prompt window.
            // provides an interface for managing both the window and viewModel together.
            var promptManager = new PromptController(settingsModel);

            // stores and retrieves all time logging data.
            var logRepo = new RavenLogRepository(@"E:\TimeLogger\");

            // provides a testable means of getting the current time.
            var clock = new Clock();

            // manages the current day.
            var dayTracker = new LogTracker(logRepo);
            
            // manages windows events to ensure the application can handle lock and sleep.
            var activityTracker = new ActivityTracker(clock);

            // attempts to orchestrate the whole application.
            var mediator = new Mediator(settingsModel, promptManager, dayTracker, clock);

            var welcomeViewModel = new WelcomeViewModel(mediator, clock);
            var loggerViewModel = new LoggerViewModel(mediator, logRepo, clock);

            var welcomeWindow = new WelcomeWindow(welcomeViewModel);
            var loggerWindow = new LoggerWindow(loggerViewModel);

            mediator.Initialise(welcomeWindow, loggerWindow);
            mediator.ShowWelcomeScreen();

            //initialise tasktray
            //load welcome screen (allows entering start time and settings)
            //start timer
            //on timer, load alert
            //alert allows snoozing upto x
            //on continue, hide self and show log screen
            //on complete, hide log screen and restart timer
        }
    }
}
