using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TimeLogger.Wpf.Views.UserControls
{
    /// <summary>
    ///     Interaction logic for Form.xaml
    /// </summary>
    public partial class Form : UserControl
    {
        public Form()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://google.com");
        }
    }
}