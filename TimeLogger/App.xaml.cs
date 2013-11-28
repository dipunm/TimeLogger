﻿using System.Windows;
using TimeLogger.Domain.Data;
using TimeLogger.Domain.OfficeManager;
using TimeLogger.Domain.UI;
using TimeLogger.Domain.Utils;
using TimeLogger.Testing;
using TimeLogger.Testing.TestObjects;
using TimeLogger.ViewModels;
using TimeLogger.Views.Windows;
using TimeLogger.Windows;

namespace TimeLogger
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            //////////
            // READY!
            //////////
            //utils
            var clock = new TestClock(); //new Clock();
            var timerFactory = new TestTimerFactory(clock);//new TimerFactory(clock);
            var userTracker = new WindowsUserTracker();

            //repo
            //var storage = new RavenBasedWorkRepository(@"E:\TimeLogger\");
            var storage = new RavenBasedWorkRepository();
            
            //////////
            // SET!
            //////////
            //Sam!
            var officeManager = new OfficeManager(timerFactory, clock, storage, userTracker);

            //UI
            var consumer = new UIConsumer(clock,
                
                new DialogController<PromptViewModel>(
                    () => Dispatcher.Invoke(() => new PromptWindow()), 
                    new PromptViewModel()),
                
                new DialogController<WelcomeViewModel>(
                    () => Dispatcher.Invoke(() => new WelcomeWindow()), 
                    new WelcomeViewModel(clock)),
                
                new DialogController<LoggerViewModel>(
                    () => Dispatcher.Invoke(() => new LoggerWindow()), 
                    new LoggerViewModel())
            );

            //////////
            // GO!
            //////////
            officeManager.ClockIn(consumer);
            

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