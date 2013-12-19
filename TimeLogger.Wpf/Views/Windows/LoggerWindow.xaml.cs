using System;
using System.Windows;
using MVVM.Extensions;
using TimeLogger.UserInterface.Core;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Wpf.Views.Windows
{
    /// <summary>
    ///     Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LoggerWindow : Window, IViewModelHandler<LoggerViewModel>, IDialog
    {
        public LoggerWindow()
        {
            InitializeComponent();
        }

        public void SetViewModel(LoggerViewModel model)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    DataContext = model;
                }));
            model.SetFinishedAction(Finished);
        }

        private void Finished()
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    DialogResult = true;
                    Hide();
                }));
        }
    }
}