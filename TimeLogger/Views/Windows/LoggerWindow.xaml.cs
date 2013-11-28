using System.Windows;
using TimeLogger.MVVM;
using TimeLogger.ViewModels;

namespace TimeLogger.Windows
{
    /// <summary>
    ///     Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LoggerWindow : Window, IViewModelHandler<LoggerViewModel>
    {
        public LoggerWindow()
        {
            InitializeComponent();
        }

        public void SetViewModel(LoggerViewModel model)
        {
            Dispatcher.Invoke(() =>
            {
                DataContext = model;
            });
            model.SetFinishedAction(Finished);
        }

        private void Finished()
        {
            Dispatcher.Invoke(() =>
            {
                DialogResult = true;
                Hide();
            });
        }
    }
}