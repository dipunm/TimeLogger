using System;

namespace TimeLogger.Domain.UI
{
    public class Timings
    {
        public TimeSpan SleepAmount { get; set; }
        public TimeSpan SnoozeAmount { get; set; }
        public TimeSpan SnoozeLimit { get; set; }
    }
}