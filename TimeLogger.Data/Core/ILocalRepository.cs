using System;
using System.Collections.Generic;
using TimeLogger.Cache.Core;

namespace TimeLogger.Data.Core
{
    public interface ILocalRepository
    {
        void AddLog(WorkLog log);
        IList<WorkLog> GetLogsForDate(DateTime date);
        void RemoveLog(WorkLog log);
    }
}