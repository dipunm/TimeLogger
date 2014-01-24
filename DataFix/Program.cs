using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Main;
using TimeLogger.Tempo.Domain;

namespace DataFix
{
    class Program
    {
        static void Main(string[] args)
        {
            var storage = new RavenTempStorage(@"C:\Timer\");
            storage.Initialise();
            var proxy = new RestApiProxy(new Uri("https://jira:8443/"));
            var logs = storage.GetAllLogs("ALL");
            logs = logs.Where(l => l.TicketCodes != null && l.TicketCodes.Contains("LL-2")).ToList();
            var session = proxy.GetSessionToken("dmistry", "MREMachine4");
            foreach (var workLog in logs)
            {
                workLog.TicketCodes = new List<string>() {"LADS-2"};
                proxy.AddWorkLog(workLog, session);
            }
        }
    }
}
