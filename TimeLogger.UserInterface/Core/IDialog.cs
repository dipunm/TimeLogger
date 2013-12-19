using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLogger.UserInterface.Core
{
    public interface IDialog : IFrameworkElement
    {
        bool? ShowDialog();
    }
}
