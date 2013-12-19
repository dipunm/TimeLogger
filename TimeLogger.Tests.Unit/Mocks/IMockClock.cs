using System;
using TimeLogger.Utils.Core;

namespace TimeLogger.Tests.Unit.Mocks
{
    public interface IMockClock : IClock
    {
        TimeSpan TickAccuracy { get; }
        void SetTime(DateTime time);
        event Action<DateTime> TimeChanged;
    }
}