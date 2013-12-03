using System;

namespace TimeLogger.Utils.Core
{
    public interface IClock
    {
        DateTime Now();
    }
}