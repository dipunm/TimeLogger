using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Models;

namespace TimeLogger.Services
{
    public interface IJiraService
    {
        IList<WorkTicket> Find(string searchTerm);
        WorkTicket GetByTicketCode(string ticketCode);
    }
}
