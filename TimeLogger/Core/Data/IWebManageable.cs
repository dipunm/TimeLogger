using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLogger.Core.Data
{
    public interface IWebManageable
    {
        Uri GetManagerUrl();
    }
}
