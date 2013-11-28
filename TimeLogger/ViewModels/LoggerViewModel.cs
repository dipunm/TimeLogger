using System;
using System.Collections.Generic;
using TimeLogger.Core.Data;
using TimeLogger.Core.Utils;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
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
        
        public void HandleWorkLog(WorkLog workLog)
        {
            _loggedWork.Add(workLog);
            Stats.NumberOfMinutesLogged += workLog.Minutes;
            if (Stats.PercentageComplete >= 1)
            {
                _submitWorkAction.Invoke(_loggedWork);
                _loggedWork.Clear();
                if(Finished != null)
                    Finished.Invoke();
            }
        }

        public void Reset(int minutesToLog)
        {
            Entry.ResetProperties();
            Stats.ResetStats(minutesToLog);
        }

        public void SetCompleteAction(Action<IList<WorkLog>> submitWorkAction)
        {
            _submitWorkAction = submitWorkAction;
        }

        public event Action Finished;
    }
}