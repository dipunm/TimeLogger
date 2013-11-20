using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Models
{
    public class WorkTicket
    {
        public string TicketCode { get; set; }
        public IList<string> Tags { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IList<string> Comments { get; set; }
        public Uri Url { get; set; }
        public string Status { get; set; }
    }
}
