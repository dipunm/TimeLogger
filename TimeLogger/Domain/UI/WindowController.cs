using System.Windows;

namespace TimeLogger.Domain.UI
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
            Window.Dispatcher.Invoke(() => Window.Hide());
        }

        public void ShowWindow()
        {
            Window.Dispatcher.Invoke(() => Window.Show());
        }
    }
}