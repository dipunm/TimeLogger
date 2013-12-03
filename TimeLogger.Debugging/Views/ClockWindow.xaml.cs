using System.Windows;

namespace TimeLogger.Debugging.Views
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow : Window
    {
        public ClockWindow()
        {
            InitializeComponent();
        }

        private void CloseAction(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
