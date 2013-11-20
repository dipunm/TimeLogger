using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Domain
{
    public interface ITimer
    {
        double Interval { get; set; }
        bool AutoReset { get; set; }
        bool Enabled { get; set; }
        void Start();
        void Stop();
    }
}
