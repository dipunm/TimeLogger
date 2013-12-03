using System;
using TimeLogger.Utils.Core;

namespace TimeLogger.Utils.Domain
{
    public class Clock : IClock
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}