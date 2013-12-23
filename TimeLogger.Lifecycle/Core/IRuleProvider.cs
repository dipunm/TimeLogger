using System;

namespace TimeLogger.Wpf.ViewModels
{
    public interface IRuleProvider
    {
        Rules FetchTimings();
        DateTime GetStartTime();
        DateTime GetEndTime();
        void Exit();
    }
}