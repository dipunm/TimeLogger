using TimeLogger.Data.Core;

namespace TimeLogger.Tempo.Core
{
    public interface ITempoProxy
    {
        string GetSessionToken();
        bool IsValidSessionToken(string sessionToken);
        void AddWorkLog(WorkLog work);
    }
}