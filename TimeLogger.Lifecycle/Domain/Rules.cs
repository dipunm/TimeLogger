using System;
using System.Collections.Generic;

namespace TimeLogger.Wpf.ViewModels
{
    public struct Rules
    {
        public TimeSpan SleepDuration { get; set; }
        public TimeSpan SnoozeDuration { get; set; }
        public TimeSpan MaximumSnoozeDuration { get; set; }
        public List<string> LoggingTicketCodes { get; set; }
    }
}