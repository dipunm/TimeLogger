﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MVVM.Extensions;
using TimeLogger.Main.ViewModels;

namespace TimeLogger.Main.Views.Windows
{
    /// <summary>
    /// Interaction logic for TempoClientWindow.xaml
    /// </summary>
    public partial class TempoClientWindow : Window, IViewModelHandler<TempoViewModel>
    {
        private TempoViewModel ViewModel { get { return DataContext as TempoViewModel; } }
        public TempoClientWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var password = Password.Password;
            if (ViewModel.LoginAction.CanExecute(password))
                ViewModel.LoginAction.Execute(password); 
        }

        public void SetViewModel(TempoViewModel model)
        {
            DataContext = model;
        }

        private void TempoClientWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.RefreshAction.CanExecute(null))
                ViewModel.RefreshAction.Execute(null); 
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel.RefreshAction.CanExecute(null))
                ViewModel.RefreshAction.Execute(null);
        }
    }
}
