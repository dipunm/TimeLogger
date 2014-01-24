using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MVVM.Extensions;
using TimeLogger.Main.Views.Windows;

namespace TimeLogger.Wpf.ViewModels
{
    public class TaskTrayViewModel : ObservableObject
    {
        private enum Functions { ClockIn = 1, LogWork = 2, ClockOut = 4, Exit = 8 }

        private readonly IRuleProvider _ruleProvider;
        private readonly ITaskMasterFactory _taskMasterFactory;
        private readonly Application _application;
        private ITaskMaster _taskMaster;
        protected List<MenuItem> ExtraMenuItems;
        private State _lastUsedState;
        public ObservableCollection<MenuItem> MenuItems { get; private set; }
        private Dictionary<Functions, MenuItem> FunctionalMenu { get; set; }
        private readonly SynchronizationContext UIContext;
        private bool _clocking;
        private bool _exiting;

        private bool Clocking
        {
            get { return _clocking; }
            set { _clocking = value; UIContext.Send(_ => RefreshMenu(), null); }
        }

        private bool Exiting
        {
            get { return _exiting; }
            set { _exiting = value; UIContext.Send(_ => RefreshMenu(), null); }
        }

        public TaskTrayViewModel(IRuleProvider ruleProvider, ITaskMasterFactory taskMasterFactory, Application application)
        {
            _ruleProvider = ruleProvider;
            _taskMasterFactory = taskMasterFactory;
            _application = application;
            ExtraMenuItems = new List<MenuItem>();
            MenuItems = new ObservableCollection<MenuItem>();
            FunctionalMenu = new Dictionary<Functions, MenuItem>()
                {
                    {
                        Functions.ClockIn, new MenuItem()
                            {
                                Header = "Clock In",
                                Command = new DelegateCommand(ClockIn)
                            }
                    },
                    {
                        Functions.LogWork, new MenuItem()
                            {
                                Header = "Log Work",
                                Command = new DelegateCommand(LogTime)
                            }
                    },
                    {
                        Functions.ClockOut, new MenuItem()
                            {
                                Header = "Clock Out",
                                Command = new DelegateCommand(ClockOut)
                            }
                    },
                    {
                        Functions.Exit, new MenuItem()
                            {
                                Header = "Exit",
                                Command = new DelegateCommand(Exit)
                            }
                    }
                };
            UIContext = SynchronizationContext.Current;
            RefreshMenu(State.Uninitialised);
        }

        private void RefreshMenu()
        {
            RefreshMenu(_lastUsedState);
        }

        private void RefreshMenu(State state)
        {
            MenuItems.Clear();
            foreach (var item in ExtraMenuItems)
            {
                MenuItems.Add(item);   
            }

            var availableFunctions = GenerateAvailableFunctions(state);
            foreach (var func in availableFunctions)
            {
                MenuItems.Add(FunctionalMenu[func]);
            }
            _lastUsedState = state;
        }

        private List<Functions> GenerateAvailableFunctions(State state)
        {
            var availableFunctions = new List<Functions>();
            switch (state)
            {
                case State.Active:
                    availableFunctions.Add(Functions.ClockOut);
                    availableFunctions.Add(Functions.Exit);
                    break;
                case State.Sleeping:
                    availableFunctions.Add(Functions.LogWork);
                    availableFunctions.Add(Functions.ClockOut);
                    availableFunctions.Add(Functions.Exit);
                    break;
                case State.Uninitialised:
                default:
                    availableFunctions.Add(Functions.ClockIn);
                    availableFunctions.Add(Functions.Exit);
                    break;
            }

            if (Clocking)
            {
                availableFunctions.Remove(Functions.ClockIn);
                availableFunctions.Remove(Functions.ClockOut);
                availableFunctions.Remove(Functions.LogWork);
            }
            if (Exiting)
            {
                availableFunctions.Remove(Functions.ClockIn);
                availableFunctions.Remove(Functions.ClockOut);
                availableFunctions.Remove(Functions.LogWork);
                availableFunctions.Remove(Functions.Exit);
            }
            return availableFunctions;
        }        

        private void ClockIn()
        {
            Clocking = true;
            new Action(() =>
                {
                    Rules rules = _ruleProvider.FetchTimings();
                    DateTime startTime = _ruleProvider.GetStartTime();
                    _taskMaster = _taskMasterFactory.CreateInstance();
                    _taskMaster.StateChanged += TaskMasterOnStateChanged;
                    _taskMaster.SetRules(rules);
                    Clocking = false;
                    _taskMaster.Begin(startTime);
                })
            .BeginInvoke(null, null);
        }

        private void TaskMasterOnStateChanged(object sender, StateChangedEventArgs stateChangedEventArgs)
        {
            UIContext.Send(_ => RefreshMenu(stateChangedEventArgs.State), null);
        }

        private void ClockOut()
        {
            ClockOut(null);
        }

        private void ClockOut(AsyncCallback onComplete)
        {
            Clocking = true;
            if (_taskMaster != null)
            {
                var tm = _taskMaster;
                _taskMaster = null;
                new Action(() =>
                    {
                        tm.Finalise(_ruleProvider.GetEndTime());
                        Clocking = false;
                    }).BeginInvoke(onComplete, null);
            }
            else
            {
                onComplete(null);
            }
        }

        private void Exit()
        {
            Exiting = true;
            _ruleProvider.Exit();
            ClockOut(_ => UIContext.Send(__ => _application.Shutdown(), null));
        }

        private void LogTime()
        {
            _taskMaster.LogAccumulatedTime();
        }

        public void AddHttpItem(string name, Uri url)
        {
            ExtraMenuItems.Add(new MenuItem()
                {
                    Header = name,
                    Command = new DelegateCommand(() => Process.Start(url.AbsoluteUri))
                });
            RefreshMenu();
        }

        public void AddWindowItem(string name, Window window)
        {
            ExtraMenuItems.Add(new MenuItem
                {
                    Header = name,
                    Command = new DelegateCommand(() => window.Dispatcher.BeginInvoke(new Action(() => window.Show())))
                });
            RefreshMenu();
        }
    }
}
