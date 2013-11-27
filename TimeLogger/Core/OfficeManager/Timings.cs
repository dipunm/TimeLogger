using System;

namespace TimeLogger.Core.OfficeManager
{
    public class Timings
    {
        public TimeSpan SleepAmount { get; set; }
        public TimeSpan SnoozeAmount { get; set; }
        public TimeSpan SnoozeLimit { get; set; }
    }
}