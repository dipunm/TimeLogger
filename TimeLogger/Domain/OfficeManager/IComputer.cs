using System;

namespace TimeLogger.Domain.Computer
{
    public delegate void ComputerEventHandler(IComputer sender);
    public interface IComputer
    {
        DateTime? LastEnded { get; }
        DateTime LastBegun { get; }
        event ComputerEventHandler UserLeft;
        event ComputerEventHandler UserReturned;
    }
}