using System;
using System.Windows.Input;

namespace TimeLogger.MVVM
{
    public class DelegateCommand : ICommand
    {
        private readonly bool _canExecute;
        private readonly Action<object> _command;

        public DelegateCommand(Action command, bool canExecute = true)
        {
            _canExecute = canExecute && command != null;
            _command = _ => { if (command != null) command(); };
        }

        public DelegateCommand(Action<object> command, bool canExecute = true)
        {
            _canExecute = canExecute && command != null;
            _command = o => { if (command != null) command(o); };
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _command(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}