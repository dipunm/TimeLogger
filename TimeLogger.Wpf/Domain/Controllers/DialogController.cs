using System;
using System.Windows;

namespace TimeLogger.Wpf.Domain.Controllers
{
    public class DialogController<TViewModel> : ViewController<TViewModel>
    {
        private readonly Func<Window> _createWindow;

        public DialogController(Func<Window> createWindow, TViewModel viewModel) : base(viewModel)
        {
            _createWindow = createWindow;
        }

        public bool? ShowDialog()
        {
            var window = _createWindow();
            ApplyViewModel(window);
            bool? returnCode = null;
            window.Dispatcher.Invoke(new Action(() => returnCode = window.ShowDialog()));
            return returnCode;
        }
    }
}