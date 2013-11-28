using System.Windows;
using TimeLogger.ViewModels;

namespace TimeLogger.Windows
{
    /// <summary>
    ///     Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private WelcomeViewModel ViewModel
        {
            get { return DataContext as WelcomeViewModel; }
        }

        public override void EndInit()
        {
            base.EndInit();
            ViewModel.OnConfirm(() => Dispatcher.Invoke(() =>
                {
                    DialogResult = true;
                    Hide();
                }));
        }
    }
}