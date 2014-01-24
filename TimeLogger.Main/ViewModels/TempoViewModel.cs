using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MVVM.Extensions;
using TimeLogger.Cache.Core;
using TimeLogger.Tempo.Core;

namespace TimeLogger.Main.ViewModels
{
    public class TempoViewModel : ObservableObject
    {
        private readonly ITempStorage _storage;
        private readonly ITempoProxy _proxy;
        private string _userName;
        private object _session;

        public object ProxySession
        {
            get { return _session; }
            set
            {
                _session = value;
                OnPropertyChanged("LoggedIn");
            }
        }

        public TempoViewModel(ITempStorage storage, ITempoProxy proxy)
        {
            _storage = storage;
            _proxy = proxy;
            WorkLogGroups = new ObservableCollection<WorkLogGroup>();
            LoginAction = new DelegateCommand(Login);
            LogoutAction = new DelegateCommand(Logout);
            RefreshAction = new DelegateCommand(Refresh);
            SubmitAction = new DelegateCommand(Submit);
            Refresh();
        }

        public TempoViewModel()
        {
        }

        private void Login(object parameter)
        {
            var password = parameter as string;
            var session = _proxy.GetSessionToken(UserName, password);
            if (!_proxy.IsValidSessionToken(session))
                return;

            ProxySession = session;
        }

        private void Logout()
        {
            ProxySession = null;
        }

        public void Refresh()
        {
            var sessions = _storage.GetNonArchivedSessionKeys();
            WorkLogGroups.Clear();
            foreach (var session in sessions)
            {
                var logs = _storage.GetAllLogs(session);
                var group = new WorkLogGroup()
                    {
                        GroupName = session,
                        TimeLogged = TimeSpan.FromMinutes(logs.Where(l => l.TicketCodes != null || l.TicketCodes.Count > 0).Sum(l => l.Minutes)),
                        Date = logs.First().Date
                    };
                WorkLogGroups.Add(group);
            }
        }

        public void Submit(object groupName)
        {
            if (!_proxy.IsValidSessionToken(ProxySession))
            {
                Logout();
            }

            foreach (var log in _storage.GetAllLogs(groupName as string))
            {
                _proxy.AddWorkLog(log, ProxySession);
            }

            _storage.Archive(groupName as string);
            Refresh();
        }

        public ICommand LoginAction { get; private set; }
        public ICommand LogoutAction { get; private set; }
        public ICommand RefreshAction { get; private set; }
        public ICommand SubmitAction { get; private set; }

        public bool LoggedIn
        {
            get { return ProxySession != null; }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public ObservableCollection<WorkLogGroup> WorkLogGroups { get; set; }
    }

    public class WorkLogGroup
    {
        public string GroupName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeLogged { get; set; }
    }
}