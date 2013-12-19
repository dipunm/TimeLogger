using System;
using System.Windows;
using TimeLogger.UserInterface.Core;

namespace TimeLogger.Wpf.Domain.Controllers
{
    public class DialogController<TViewModel> : ViewController<TViewModel>
    {
        private readonly Func<IDialog> _createWindow;

        public DialogController(Func<IDialog> createWindow, TViewModel viewModel)
            : base(viewModel)
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