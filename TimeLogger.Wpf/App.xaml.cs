using System;
using System.Runtime.InteropServices;
using System.Windows;
using Fclp;
using TimeLogger.Core.Utils;
using TimeLogger.Domain.Data;
using TimeLogger.Domain.OfficeManager;
using TimeLogger.Domain.UI;
using TimeLogger.Domain.Utils;
using TimeLogger.Testing;
using TimeLogger.Testing.TestObjects;
using TimeLogger.ViewModels;
using TimeLogger.Views.Other;
using TimeLogger.Views.Windows;
using TimeLogger.Windows;

namespace TimeLogger
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _runInTestMode = false;
        private bool _runInMemMode = false;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var success = ParseArguments(e.Args);
            if (!success)
            {
                Shutdown();
                return;
            }

            //////////
            // READY!
            //////////
            //utils
            IClock clock = new Clock();
            ITimerFactory timerFactory = new TimerFactory(clock);
            IUserTracker userTracker = new WindowsUserTracker();

            //test overrides
            if (_runInTestMode)
            {
                clock = new TestClock();
                timerFactory = new TimerFactory(clock);
            }

            //repo
            var storage = _runInMemMode ? 
                new RavenBasedWorkRepository() : 
                new RavenBasedWorkRepository(@"E:\TimeLogger\");

            storage.Initialise();
            
            //////////
            // SET!
            //////////
            //Sam!
            var officeManager = new OfficeManager(timerFactory, clock, storage, userTracker);

            //UI
            var consumer = new UIConsumer(clock, timerFactory,
                
                new DialogController<PromptViewModel>(
                    () => (Window) Dispatcher.Invoke(new Func<Window>(() => new PromptWindow())), 
                    new PromptViewModel()),
                
                new DialogController<WelcomeViewModel>(
                    () => (Window) Dispatcher.Invoke(new Func<Window>(() => new WelcomeWindow())), 
                    new WelcomeViewModel(clock)),
                
                new DialogController<LoggerViewModel>(
                    () => (Window) Dispatcher.Invoke(new Func<Window>(() => new LoggerWindow())), 
                    new LoggerViewModel())
            );

            //////////
            // GO!
            //////////
            var taskTray = new MainTray();
            var taskTrayViewModel = new TaskTrayViewModel(this, officeManager, consumer);
            taskTrayViewModel.AddHttpItem("Management Studio", storage.GetManagerUrl());
            taskTray.DataContext = taskTrayViewModel;
        }

        private bool ParseArguments(string[] args)
        {
            var parser = new FluentCommandLineParser();
            parser.Setup<bool>("testmode")
                  .Callback(val => _runInTestMode = val)
                  .SetDefault(false)
                  .WithDescription("Runs the application in development mode where you are the clock.");

            parser.Setup<bool>("memorymode")
                  .Callback(val => _runInMemMode = val)
                  .SetDefault(false)
                  .WithDescription(
                      "Runs the application in memory mode where the database does not persist to file. All data is lost when the application is closed.");

            var earlyClose = true;
            //parser.SetupHelp("?", "help")
            //      .Callback(message =>
            //          {
            //              WriteToConsole(message);
            //              earlyClose = false;
            //              this.Shutdown();
            //          });

            parser.IsCaseSensitive = false;

            parser.Parse(args);
            return earlyClose;
        }
    }
}