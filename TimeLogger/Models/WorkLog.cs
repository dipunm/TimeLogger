using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Models
{
    public class WorkLog
    {
        public string Comment { get; set; }

        public int Minutes { get; set; }

        public List<string> TicketCodes { get; set; }

        public DateTime Date { get; set; }
    }
}
