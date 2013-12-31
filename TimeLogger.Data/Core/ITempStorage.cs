using System.Collections.Generic;
using TimeLogger.Cache.Core;

namespace TimeLogger.Main
{
    public interface ITempStorage
    {
        List<WorkLog> GetAllLogs(string sessionKey);
        void AddRange(IEnumerable<WorkLog> logs, string sessionKey);
        string GenerateSessionKey();
        IEnumerable<string> GetNonArchivedSessionKeys();
    }
}