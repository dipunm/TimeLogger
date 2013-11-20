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
    public partial class WelcomeWindow : Window
    {
        private readonly WelcomeViewModel _viewModel;
        public WelcomeWindow(WelcomeViewModel viewModel)
        {
            this.DataContext = _viewModel = viewModel;
            InitializeComponent();
        }

        public void Show(Models.Settings _settingsModel)
        {
            _viewModel.Bind(_settingsModel);
            this.Show();
        }
    }
}
