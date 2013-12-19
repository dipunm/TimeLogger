using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TimeLogger.Cache.Core;
using TimeLogger.Data.Core;
using TimeLogger.Data.Local.Domain;
using TimeLogger.Lifecycle.Core;
using TimeLogger.UserInterface.Core;
using TimeLogger.UserInterface.ViewModels;
using TimeLogger.Utils.Core;
using TimeLogger.Utils.Domain;
using TimeLogger.Wpf.Domain;
using TimeLogger.Wpf.Domain.Controllers;
using TimeLogger.Wpf.Views.Windows;

namespace TimeLogger.Wpf.ViewModels
{
    public class ApplicationContext : IApplication
    {
        private readonly Application _application;

        public ApplicationContext(Application application)
        {
            _application = application;
        }

        public void Shutdown()
        {
            _application.Shutdown();
        }

        public IComposer CreateComposer()
        {
            var clock = new Clock();
            var osTracker = new WindowsOsTracker();
            var timerFactory = new TimerFactory(clock, osTracker);

            var consumer = new UIConsumer(timerFactory,
                new DialogController<PromptViewModel>(
                    () => (IDialog)_application.Dispatcher.Invoke(new Func<IDialog>(() => new PromptWindow())),
                    new PromptViewModel()),

                new DialogController<WelcomeViewModel>(
                    () => (IDialog)_application.Dispatcher.Invoke(new Func<IDialog>(() => new WelcomeWindow())),
                    new WelcomeViewModel()),

                new DialogController<LoggerViewModel>(
                    () => (IDialog)_application.Dispatcher.Invoke(new Func<IDialog>(() => new LoggerWindow())),
                    new LoggerViewModel())
                );
            var storage = new RavenBasedLocalRepository(@"C:\TimeLogger\");
            var composer = new Composer(consumer, storage, GetTime());
            return composer;
        }

        public DateTime GetTime()
        {
            return DateTime.Now;
        }

        public DateTime GetEndTime()
        {
            return DateTime.Now;
        }
    }

    public class Composer : IComposer
    {
        private readonly ITimeLogger _consumer;
        private readonly ILocalRepository _storage;
        private readonly DateTime _startTime;

        public Composer(ITimeLogger consumer, ILocalRepository storage, DateTime startTime)
        {
            _consumer = consumer;
            _storage = storage;
            _startTime = startTime;
        }

        public bool Ending { get; set; }
        public void LogWork(DateTime targetTime)
        {
            var logs = _storage.GetLogsForDate(_startTime.Date);
            var minsLogged = logs
                .Where(l => l.TicketCodes != null && l.TicketCodes.Any())
                .Sum(l => l.Minutes);
            var timeToLog = targetTime - _startTime - TimeSpan.FromMinutes(minsLogged);
            _consumer.LogTime(timeToLog, Ending);
        }

        public ComposerState GetState()
        {
            return ComposerState.Idle;
        }
    }

    public interface ITimeLogger
    {
        List<WorkLog> LogTime(TimeSpan timeToLog, bool );
    }

}