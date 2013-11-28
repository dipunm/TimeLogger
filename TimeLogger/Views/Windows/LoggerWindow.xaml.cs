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
            var oldModel = DataContext as LoggerViewModel;
            if(oldModel != null)
                oldModel.Finished -= Finished;
    
            DataContext = model;
            model.Finished += Finished;
        }

        private void Finished()
        {
            DialogResult = true;
            Close();
        }
    }
}