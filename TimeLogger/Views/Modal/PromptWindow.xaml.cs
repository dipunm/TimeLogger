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

namespace TimeLogger.Views.Modal
{
    /// <summary>
    /// Interaction logic for Prompt.xaml
    /// </summary>
    public partial class PromptWindow : Window
    {
        private readonly PromptViewModel _viewModel;
        public PromptWindow(PromptViewModel viewModel)
        {
            this.DataContext = _viewModel = viewModel;
            InitializeComponent();
        }
    }
}
