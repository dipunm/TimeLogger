using System;

namespace TimeLogger.Models
{
    public class Settings
    {
        public TimeSpan SleepDuration { get; set; }
        public TimeSpan SnoozeDuration { get; set; }
        public TimeSpan MaxSnoozeLimit { get; set; }
        public string TimeLoggingTickets { get; set; }
    }
}
