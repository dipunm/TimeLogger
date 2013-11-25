using System;

namespace TimeLogger.Core
{
    public delegate void ActivityTrackerEventHandler(IActivityTracker sender); 
    public interface IActivityTracker
    {
        DateTime? LastEnded { get; }
        DateTime LastBegun { get; }
        event ActivityTrackerEventHandler UserLeft;
        event ActivityTrackerEventHandler UserReturned;
    }
}
