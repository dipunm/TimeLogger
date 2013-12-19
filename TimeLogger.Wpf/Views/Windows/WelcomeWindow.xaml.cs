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
    public partial class WelcomeWindow : Window, IViewModelHandler<WelcomeViewModel>, IDialog
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