using System;
using System.Windows;
using MVVM.Extensions;
using TimeLogger.Wpf.ViewModels;
using System.IO;

namespace TimeLogger.Wpf.Views.Windows
{
    /// <summary>
    ///     Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LoggerWindow : Window, IViewModelHandler<LoggerViewModel>
    {
        public LoggerWindow()
        {
            InitializeComponent();
            this.Closing += (s, e) => SaveToFile();
            LoadFromFile();
        }

        private void SaveToFile()
        {
            var content = NotesBox.Text;
            File.WriteAllText(filename, content, System.Text.Encoding.UTF8);
        }

        private const string filename = "notes.txt";

        private void LoadFromFile()
        {
            if (!File.Exists(filename))
                return;

            string content = File.ReadAllText(filename, System.Text.Encoding.UTF8);
            NotesBox.Text = content;
        }


        public void SetViewModel(LoggerViewModel model)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    DataContext = model;
                }));
            model.SetFinishedAction(Finished);
        }

        private void Finished()
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    DialogResult = true;
                    Hide();
                }));
        }
    }
}