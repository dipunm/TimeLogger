using System;
using TimeLogger.Core.Utils;

namespace TimeLogger.Domain.Utils
{
    public class Clock : IClock
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}