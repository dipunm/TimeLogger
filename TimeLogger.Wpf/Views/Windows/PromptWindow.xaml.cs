using System.Windows;
using TimeLogger.UserInterface.Core;

namespace TimeLogger.Wpf.Views.Windows
{
    /// <summary>
    ///     Interaction logic for Prompt.xaml
    /// </summary>
    public partial class PromptWindow : Window, IDialog
    {
        public PromptWindow()
        {
            InitializeComponent();
        }

        private void SnoozeClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void LogTimeClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}