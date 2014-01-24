using System;
using System.Collections.Generic;
using MVVM.Extensions;
using TimeLogger.Cache.Core;

namespace TimeLogger.Wpf.ViewModels
{
    public class LoggerViewModel : ObservableObject
    {
        private readonly List<WorkLog> _loggedWork;
        private Action<IList<WorkLog>> _submitWorkAction;
        
        public LoggerViewModel()
        {
            _loggedWork = new List<WorkLog>();
            Entry = new WorkEntryViewModel();
            Stats = new WorkLogStatsViewModel();
            Entry.WorklogSubmitted += HandleWorkLog;
        }

        public WorkEntryViewModel Entry { get; private set; }
        public WorkLogStatsViewModel Stats { get; private set; }

        public ICollection<WorkLog> Logs
        {
            get { return _loggedWork; }
        }

        public void HandleWorkLog(WorkLog workLog)
        {
            _loggedWork.Add(workLog);
            Stats.NumberOfMinutesLogged += workLog.Minutes;
            if (Stats.PercentageComplete >= 1)
            {
                if (Finished != null)
                    Finished.Invoke();
            }
        }

        public void Reset(int minutesToLog)
        {
            _loggedWork.Clear();
            Entry.ResetProperties();
            Stats.ResetStats(minutesToLog);
        }

        public void SetFinishedAction(Action finished)
        {
            Finished = finished;
        }

        private Action Finished;

        public void Update(int minutesToLog)
        {
            Stats.UpdateDuration(minutesToLog);
        }
    }
}