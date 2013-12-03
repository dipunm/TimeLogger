using System;

namespace TimeLogger.Utils.Core
{
    public interface ITimeTracker
    {
        void Start();
        TimeSpan Stop();
    }
}