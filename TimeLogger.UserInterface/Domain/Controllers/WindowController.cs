using System;
using System.Windows;
using TimeLogger.UserInterface.Core;

namespace TimeLogger.Wpf.Domain.Controllers
{
    public class WindowController<TViewModel> : ViewController<TViewModel>
    {
        private readonly IFrameworkElement _window;

        public WindowController(IFrameworkElement window, TViewModel viewModel) : base(viewModel)
        {
            _window = window;
            ApplyViewModel(_window);
        }

        public Window Window
        {
            get { return _window as Window; }
        }
        
        public void HideWindow()
        {
            Window.Dispatcher.Invoke(new Action(() => Window.Hide()));
        }

        public void ShowWindow()
        {
            Window.Dispatcher.Invoke(new Action(() => Window.Show()));
        }
    }
}