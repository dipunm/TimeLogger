using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Core.Utils;

namespace TimeLogger.Domain.Utils
{
    public class TimerFactory : ITimerFactory
    {
        public ITimer CreateTimer()
        {
            return new Timer();
        }
    }
}
