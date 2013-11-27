using System;
using System.Collections.Generic;

namespace TimeLogger.Core.Data
{
    public class WorkLog
    {
        public string Comment { get; set; }
        public int Minutes { get; set; }
        public List<string> TicketCodes { get; set; }
        public DateTime Date { get; set; }
    }
}
