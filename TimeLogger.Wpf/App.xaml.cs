using System;
using System.Windows;
using Fclp;
using TimeLogger.Cache.Core;
using TimeLogger.Data.Local.Domain;
using TimeLogger.Data.Remote.Domain;
using TimeLogger.Debugging.TestObjects;
using TimeLogger.Lifecycle.Core;
using TimeLogger.Lifecycle.Domain;
using TimeLogger.UserInterface.Core;
using TimeLogger.UserInterface.ViewModels;
using TimeLogger.Utils.Core;
using TimeLogger.Utils.Domain;
using TimeLogger.Wpf.Domain;
using TimeLogger.Wpf.Domain.Controllers;
using TimeLogger.Wpf.ViewModels;
using TimeLogger.Wpf.Views.Other;
using TimeLogger.Wpf.Views.Windows;

namespace TimeLogger.Wpf
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
            IClock clock = _runInTestMode ? (IClock) new TestClock() : new Clock();
            IOsTracker osTracker = new WindowsOsTracker();
            ITimerFactory timerFactory = new TimerFactory(clock, osTracker);

            //repo
            var storage = _runInMemMode ? 
                new RavenBasedLocalRepository() : 
                new RavenBasedLocalRepository(@"E:\TimeLogger\");

            storage.Initialise();
            
            //////////
            // SET!
            //////////
            //Sam!
            var officeManager = new OfficeManager(timerFactory, clock, storage);
            
            //UI
            var consumer = new UIConsumer(clock, timerFactory,
                
                new DialogController<PromptViewModel>(
                    () => (IDialog)Dispatcher.Invoke(new Func<IDialog>(() => new PromptWindow())), 
                    new PromptViewModel()),
                
                new DialogController<WelcomeViewModel>(
                    () => (IDialog)Dispatcher.Invoke(new Func<IDialog>(() => new WelcomeWindow())), 
                    new WelcomeViewModel()),
                
                new DialogController<LoggerViewModel>(
                    () => (IDialog)Dispatcher.Invoke(new Func<IDialog>(() => new LoggerWindow())), 
                    new LoggerViewModel())
            );

            var stateMachine = new OfficeManagerStateMachine(officeManager, consumer);


            TempoArchive.BypassSslErrors();
            var archive = new TempoArchive();

            storage.SendAllToArchive(archive);

            //////////
            // GO!
            //////////
            var taskTray = new MainTray();
            var taskTrayViewModel = new TaskTrayViewModel(
                new ApplicationContext(this)
            );
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

            //var earlyClose = true;
            //parser.SetupHelp("?", "help")
            //      .Callback(message =>
            //          {
            //              WriteToConsole(message);
            //              earlyClose = false;
            //              this.Shutdown();
            //          });

            parser.IsCaseSensitive = false;

            parser.Parse(args);

            //return earlyClose;
            return true;
        }
    }
}