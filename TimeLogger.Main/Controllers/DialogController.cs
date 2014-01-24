using System;
using System.Windows;

namespace TimeLogger.Wpf.Domain.Controllers
{
    public class DialogController<TViewModel> : ViewController<TViewModel>
    {
        private readonly Func<Window> _createWindow;
        private Window _activeWindow;
        private object _activeWindowLock = new object();

        public DialogController(Func<Window> createWindow, TViewModel viewModel) : base(viewModel)
        {
            _createWindow = createWindow;
        }

        public bool? ShowDialog()
        {
            if (_activeWindow == null)
            {
                var window = _createWindow();
                _activeWindow = window;
                ApplyViewModel(window);
                bool? returnCode = null;
                window.Dispatcher.Invoke(new Action(() => returnCode = window.ShowDialog()));
                _activeWindow = null;
                return returnCode;
            }
            else
            {
                return false;
            }
        }

        public void ForceCloseOnExecutingThread()
        {
            if (_activeWindow != null)
            {
                Window window;
                lock (_activeWindowLock)
                {
                    window = _activeWindow;
                }

                if (window != null)
                {
                    window.Dispatcher.Invoke(new Action(() => _activeWindow.Close()));
                }
            }
        }
    }
}