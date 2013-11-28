using System;

namespace TimeLogger.Core.Utils
{
    public interface ITimeTracker
    {
        void Start();
        TimeSpan Stop();
    }
}