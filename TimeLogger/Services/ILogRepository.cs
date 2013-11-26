using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Domain.Data;
using TimeLogger.Models;

namespace TimeLogger.Services
{
    public interface ILogRepository
    {
        void AddLog(WorkLog log);
        IList<WorkLog> GetAllLogs();
        void RemoveLog(WorkLog log);
    }
}
