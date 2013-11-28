using System.Windows;

namespace TimeLogger.Domain.UI
{
    public class WindowViewModelController<TViewModel>
    {
        private readonly TViewModel _viewModel;
        private readonly Window _window;

        public WindowViewModelController(Window window, TViewModel viewModel)
        {
            _window = window;
            _viewModel = viewModel;
            _window.DataContext = viewModel;
        }

        public Window Window
        {
            get { return _window; }
        }

        public TViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public void HideWindow()
        {
            Window.Dispatcher.Invoke(() => Window.Hide());
        }

        public void ShowWindow()
        {
            Window.Dispatcher.Invoke(() => Window.Show());
        }

        public bool? ShowDialog()
        {
            bool? returnCode = null;
            Window.Dispatcher.Invoke(() => returnCode = Window.ShowDialog());
            return returnCode;
        }
    }
}