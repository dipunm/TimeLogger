using System;
using System.Collections.Generic;

namespace TimeLogger.Cache.Core
{
    public class WorkLog
    {
        public string Comment { get; set; }
        public int Minutes { get; set; }
        public List<string> TicketCodes { get; set; }
        public DateTime Date { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchiveDate { get; set; }
    }
}