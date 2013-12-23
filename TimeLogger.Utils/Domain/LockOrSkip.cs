using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeLogger.Utils.Domain
{
    public class LockOrSkip : IDisposable
    {
        private readonly object _lockObject;

        public LockOrSkip(object lockObject)
        {
            if (Monitor.TryEnter(lockObject))
            {
                _lockObject = lockObject;
            }
        }


        public void Dispose()
        {
            if (_lockObject != null)
            {
                Monitor.Exit(_lockObject);
            }
        }
    }
}
