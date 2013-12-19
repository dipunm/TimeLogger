using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using MVVM.Extensions;
using TimeLogger.UserInterface.ViewModels;

namespace TimeLogger.Wpf.Views.Other
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainTray : TaskbarIcon, IViewModelHandler<TaskTrayViewModel>
    {
        public MainTray()
        {
            InitializeComponent();
        }

        private void MainTray_OnPreviewTrayContextMenuOpen(object sender, RoutedEventArgs e)
        {
            if(ViewModel.RefreshCommand.CanExecute(null))
                ViewModel.RefreshCommand.Execute(null);
        }

        public void SetViewModel(TaskTrayViewModel model)
        {
            ViewModel = model;
        }

        private TaskTrayViewModel ViewModel
        {
            get { return DataContext as TaskTrayViewModel; }
            set { DataContext = value; }
        }
    }
}
