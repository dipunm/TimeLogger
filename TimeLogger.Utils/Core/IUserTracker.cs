namespace TimeLogger.Utils.Core
{
    public delegate void ComputerEventHandler(IUserTracker sender);

    public interface IUserTracker
    {
        event ComputerEventHandler UserLeft;
        event ComputerEventHandler UserReturned;
    }
}