using System;

namespace TimeLogger.Core.Utils
{
    public interface IClock
    {
        DateTime Now();
    }
}