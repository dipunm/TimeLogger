using System;

namespace TimeLogger.Wpf.ViewModels
{
    public interface ITaskMaster
    {
        void SetRules(Rules rules);
        void Begin(DateTime startTime);
        void LogAccumulatedTime();
        void Finalise(DateTime endTime);
        event StateChangedEventHandler StateChanged;
    }
}