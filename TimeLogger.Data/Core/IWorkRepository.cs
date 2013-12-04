using System;
using System.Collections.Generic;

namespace TimeLogger.Cache.Core
{
    public interface IWorkRepository
    {
        void AddLog(WorkLog log);
        IList<WorkLog> GetLogsForDate(DateTime date);
        void RemoveLog(WorkLog log);
    }
}