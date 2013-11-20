using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimeLogger.ViewModels;

namespace TimeLogger.Windows
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LoggerWindow : Window
    {
        private readonly LoggerViewModel _viewModel;
        public LoggerWindow(LoggerViewModel viewModel)
        {
            this.DataContext = _viewModel = viewModel;
            InitializeComponent();
        }

        public void Show(int minutesToLog, DateTime nowTime)
        {
            _viewModel.Reset(minutesToLog, nowTime);
            this.Show();
        }
    }
}
