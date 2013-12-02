using System;
using System.Windows;
using TimeLogger.MVVM;
using TimeLogger.ViewModels;

namespace TimeLogger.Windows
{
    /// <summary>
    ///     Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window,  IViewModelHandler<WelcomeViewModel>
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private WelcomeViewModel ViewModel
        {
            get { return DataContext as WelcomeViewModel; }
        }

        public void SetViewModel(WelcomeViewModel model)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    DataContext = model;
                }));

            model.OnConfirm(() => Dispatcher.Invoke(new Action(() =>
                {
                    DialogResult = true;
                    Close();
                })));
        }
    }
}