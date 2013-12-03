using System;
using System.Windows;

namespace TimeLogger.Wpf.Domain.Controllers
{
    public class WindowController<TViewModel> : ViewController<TViewModel>
    {
        private readonly Window _window;

        public WindowController(Window window, TViewModel viewModel) : base(viewModel)
        {
            _window = window;
            ApplyViewModel(_window);
        }

        public Window Window
        {
            get { return _window; }
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