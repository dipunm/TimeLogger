using TimeLogger.Core.Data;

namespace TimeLogger.Core.Tempo
{
    public interface ITempoProxy
    {
        string GetSessionToken();
        bool IsValidSessionToken(string sessionToken);
        void AddWorkLog(WorkLog work);
    }
}