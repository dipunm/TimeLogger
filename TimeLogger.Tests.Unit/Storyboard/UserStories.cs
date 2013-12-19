using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Moq;
using NUnit.Framework;
using TimeLogger.Data.Local.Domain;
using TimeLogger.Lifecycle.Domain;
using TimeLogger.Tests.Unit.Mocks;
using TimeLogger.UserInterface.Core;
using TimeLogger.UserInterface.ViewModels;
using TimeLogger.Utils.Core;
using TimeLogger.Utils.Domain;
using TimeLogger.Wpf.Domain;
using TimeLogger.Wpf.Domain.Controllers;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Tests.Unit.Storyboard
{
    public enum ContextMenu
    {
        ClockIn = 0, ClockOut = 1, 
    }
    [TestFixture]
    public class UserStories
    {
        private TaskTrayViewModel _taskTrayViewModel;
        private MockClock _clock;
        private Mock<ITimerFactory> _timerFactory;
        private WindowsOsTracker _osTracker;
        private RavenBasedLocalRepository _storage;
        private OfficeManager _officeManager;
        private Mock<IDialog> _promptWindow;
        private Mock<IDialog> _welcomeWindow;
        private Mock<IDialog> _loggerWindow;
        private UIConsumer _consumer;
        private Mock<IApplication> _application;
        private PromptViewModel _promptViewModel;
        private WelcomeViewModel _welcomeViewModel;
        private LoggerViewModel _LoggerViewModel;
        private Mock<ITimer> _workTimer;
        private Mock<ITimer> _snoozeTimer;

        [SetUp]
        public void SetUp()
        {
            _clock = new MockClock();
            _timerFactory = new Mock<ITimerFactory>();
            
            _timerFactory.Setup(f => f.CreateTimeTracker())
                .Returns(() => new TimeTracker(_clock));
            
            _workTimer = new Mock<ITimer>();
            _snoozeTimer = new Mock<ITimer>();
            int timerCount = 0;
            _timerFactory.Setup(f => f.CreateTimer())
                .Returns(() =>
                    {
                        switch (timerCount++)
                        {
                            case 0:
                                return _workTimer.Object;
                            case 1:
                                return _snoozeTimer.Object;
                            default:
                                throw new Exception(
                                    "Scope has changed. Please alter this test to understand the use of new timer");
                        }
                    });
            _osTracker = new WindowsOsTracker();

            //repo
            _storage = new RavenBasedLocalRepository();
            _storage.Initialise();

            //Sam!
            _officeManager = new OfficeManager(_timerFactory.Object, _clock, _storage, _osTracker);

            //UI
            var mockDispatcher = Dispatcher.CurrentDispatcher;
            _promptWindow = new Mock<IDialog>();
            _welcomeWindow = new Mock<IDialog>();
            _welcomeWindow.Setup(w => w.ShowDialog())
                .Callback(() => 
                    _welcomeViewModel.Initialise.Execute(_clock.Now())
                );
            _loggerWindow = new Mock<IDialog>();

            _promptWindow.Setup(w => w.Dispatcher).Returns(mockDispatcher);
            _welcomeWindow.Setup(w => w.Dispatcher).Returns(mockDispatcher);
            _loggerWindow.Setup(w => w.Dispatcher).Returns(mockDispatcher);
            
            _promptViewModel = new PromptViewModel();
            _welcomeViewModel = new WelcomeViewModel();
            _LoggerViewModel = new LoggerViewModel();

            _consumer = new UIConsumer(_clock, _timerFactory.Object,

                new DialogController<PromptViewModel>(
                    () => _promptWindow.Object,
                    _promptViewModel),

                new DialogController<WelcomeViewModel>(
                    () => _welcomeWindow.Object,
                    _welcomeViewModel),

                new DialogController<LoggerViewModel>(
                    () => _loggerWindow.Object,
                    _LoggerViewModel)
            );

            _application = new Mock<IApplication>();
            _taskTrayViewModel = new TaskTrayViewModel(_application.Object, _officeManager, _consumer);
            _taskTrayViewModel.AddHttpItem("Management Studio", _storage.GetManagerUrl());
        }

        //External sources: Time (clock), Timer, TimeTracker, storage, UI
        //Time fully mocked
        //Timer SHOULD ALSO BE FULLY MOCKED... TIMER CAN BE TESTED SEPARATELY!
        //TimeTracker can use real tracker
        //Storage mocked to in-memory
        //UI ViewModels controlled from tests directly

        [Test]
        public void Story1()
        {
            //when the welcome window shows, we enter our values
            _welcomeWindow.Setup(d => d.ShowDialog()).Callback(() =>
            {
                _welcomeViewModel.StartTime = _clock.Now();
                _welcomeViewModel.TimeLoggingTickets = "PAY-99";
                _welcomeViewModel.SnoozeDurationMins = 5;
                _welcomeViewModel.SleepDurationMins = 60;
                _welcomeViewModel.MaxSnoozeDurationMins = 15;
                //the time becomes 9:02.00
                SetTime(9, 2);
                //we hit submit
                _welcomeViewModel.Begin.Execute(null);
            })
            .Returns(true);
            
            //At first, time is 9:01.00
            SetTime(9, 1);
            //User initiates clock in
            _taskTrayViewModel.MenuItems[(int)ContextMenu.ClockIn].Command.Execute(null);
            //verify dialog was shown
            _welcomeWindow.Verify(w => w.ShowDialog());
            //verify work timer was set up
            VerifyTimerStarted();
            //When prompt shows, we snooze
            _promptWindow.Setup(p => p.ShowDialog()).Callback(() =>
            {
                //the time becomes 10:02.29
                SetTime(10,2,29);
            })
            //we snooze
            .Returns(false);
            
            //the time becomes 10:01.00
            SetTime(10, 1);
            _workTimer.Raise(t => t.Elapsed += null, _workTimer.Object);
            _promptWindow.Verify(w => w.ShowDialog());
            _workTimer.VerifySet(t => t.Duration = It.IsAny<TimeSpan>(), Times.Once);
            _workTimer.Verify(t => t.Start(), Times.Once);
            //the time becomes 10:07.29
            SetTime(10, 7, 29);
            
            //Prompt window shows
            //the time becomes 9:44.33
            //Snooze gets disabled
            //we agree to log
            //Log window opens
            //we enter some data: under limit
            //submit
            //Form gets cleared
            //We enter some data: just right
            //Form Closes
            //the time becomes 
        }

        private void VerifyTimerStarted()
        {
            _workTimer.VerifySet(t => t.Duration = It.IsAny<TimeSpan>(), Times.Once);
            _workTimer.Verify(t => t.Start(), Times.Once);
        }

        private void SetTime(int hrs, int mins = 0, int secs = 0)
        {
            _clock.SetTime(new DateTime(2013, 12, 04, hrs, mins, secs));
        }
    }
}
