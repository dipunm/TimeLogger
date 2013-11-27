namespace TimeLogger.Core.Utils
{
    public delegate void ComputerEventHandler(IUserTracker sender);
    public interface IUserTracker
    {
        event ComputerEventHandler UserLeft;
        event ComputerEventHandler UserReturned;
    }
}