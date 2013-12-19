using TimeLogger.Cache.Core;

namespace TimeLogger.Data.Core
{
    
    public interface IRemoteArchive
    {
        Reason Authenticate(string username, string password);
        Reason AddWorkLog(WorkLog work);
    }
}