using TimeLogger.Cache.Core;

namespace TimeLogger.Tempo.Core
{
    public interface ITempoProxy
    {
        object GetSessionToken(string username, string password);
        bool IsValidSessionToken(object sessionToken);
        void AddWorkLog(WorkLog work, object sessionToken);
    }
}