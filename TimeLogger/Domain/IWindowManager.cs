using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeLogger.Domain
{
    /// <summary>
    /// Manages the visibility of windows allowing soft references using strings.
    /// </summary>
    public interface IWindowManager
    {
        void Remove(string name);
        void Register<TWindow>(string name) where TWindow : Window;
        void Register(Window view, string name);
        bool IsVisible(string name);
        void Show(string name);
        void Hide(string name);
    }

}
