using TimeLogger.Utils.Core;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Main
{
    public class TaskMasterFactory : ITaskMasterFactory
    {
        private readonly IAlarmClock _alarmClock;
        private readonly ITempStorage _storage;
        private readonly ILogRetriever _logRetriever;

        public TaskMasterFactory(IAlarmClock alarmClock, ITempStorage storage, ILogRetriever logRetriever)
        {
            _alarmClock = alarmClock;
            _storage = storage;
            _logRetriever = logRetriever;
        }

        public ITaskMaster CreateInstance()
        {
            return new TaskMaster(_alarmClock, _storage, _logRetriever);
        }
    }
}