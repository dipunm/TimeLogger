using System;
using System.Windows;
using System.Windows.Threading;
using TimeLogger.Main.ViewModels;
using TimeLogger.Main.Views.Windows;
using TimeLogger.Tempo.Domain;
using TimeLogger.Utils.Core;
using TimeLogger.Utils.Domain;
using TimeLogger.Wpf.Domain.Controllers;
using TimeLogger.Wpf.ViewModels;
using TimeLogger.Wpf.Views.Other;
using TimeLogger.Wpf.Views.Windows;

namespace TimeLogger.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Main(object sender, StartupEventArgs e)
        {
            var osTracker = new WindowsOsTracker();
            var clock = new AlarmClock(osTracker);
            var storage = new RavenTempStorage(@"C:\Timer\");
            storage.Initialise();

            var logRetriever = new LogRetriever(
                new DialogController<PromptViewModel>(
                    DispatcherFunc<PromptWindow>(),
                    new PromptViewModel()
                ),
                new DialogController<LoggerViewModel>(
                    DispatcherFunc<LoggerWindow>(),
                    new LoggerViewModel()
                ),
                new TimeFactory(clock)
            );

            var taskTray = new MainTray();
            var ruleProvider = new RuleProvider(
                new DialogController<WelcomeViewModel>(
                    DispatcherFunc<WelcomeWindow>(),
                    new WelcomeViewModel(clock)
                )
            );

            var taskTrayViewModel = new TaskTrayViewModel(
                ruleProvider, 
                new TaskMasterFactory(clock, storage, logRetriever),
                this
            );
            taskTray.DataContext = taskTrayViewModel;
            taskTrayViewModel.AddHttpItem("Database", storage.GetManagementUri());
            taskTrayViewModel.AddHttpItem("Jira", new Uri("https://jira.dfc.local:8443/secure/Dashboard.jspa"));
            
            var tempoProxy = new RestApiProxy(new Uri("https://jira.dfc.local:8443/"));
            var tempoClient = new TempoClientWindow();
            tempoClient.SetViewModel(new TempoViewModel(storage, tempoProxy));
            taskTrayViewModel.AddWindowItem("Tempo", tempoClient);
        }

        private Func<Window> DispatcherFunc(Func<Window> action)
        {
            return () => (Window)Dispatcher.Invoke(new Func<Window>(action));
        }

        private Func<Window> DispatcherFunc<TWindow>() where TWindow : Window, new()
        {
            return DispatcherFunc(() => new TWindow());
        }
    }
}
