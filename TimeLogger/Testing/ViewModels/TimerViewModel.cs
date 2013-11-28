﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeLogger.MVVM;

namespace TimeLogger.Testing
{
    public class TimerViewModel : ObservableObject
    {
        private bool _inProgress;
        private bool _shouldFire;
        private TimeSpan _duration;

        public TimerViewModel()
        {
            Messages = new ObservableCollection<string>();
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                if (value.Equals(_inProgress)) return;
                _inProgress = value;
                OnPropertyChanged();
            }
        }

        public bool ShouldFire
        {
            get { return _shouldFire; }
            set
            {
                if (value.Equals(_shouldFire)) return;
                _shouldFire = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                if (value.Equals(_duration)) return;
                _duration = value;
                OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Messages { get; private set; } 

        public void AddMessage(string message, DateTime now)
        {
            Messages.Add(String.Format("{0} - {1}", message, now));
        }

        public ICommand ElapsedCommand { get; set; }

        public void OnElapsed(Action action)
        {
            ElapsedCommand = new DelegateCommand(action);
        }
    }
}