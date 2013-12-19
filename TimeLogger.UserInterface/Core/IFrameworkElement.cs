using System.Windows.Threading;

namespace TimeLogger.UserInterface.Core
{
    public interface IFrameworkElement
    {
        Dispatcher Dispatcher { get; }
        object DataContext { get; set; }
    }
}