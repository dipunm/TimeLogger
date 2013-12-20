namespace TimeLogger.Utils.Core
{
    public delegate void ComputerEventHandler(IOsTracker sender);

    public interface IOsTracker
    {
        event ComputerEventHandler UserLeft;
        event ComputerEventHandler UserReturned;
    }
}