using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace MVVM.Extensions
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

        public virtual void Execute(object parameter)
        {
            _command(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }

    public class DispatcherDelegateCommand : DelegateCommand
    {
        private readonly Dispatcher _dispatcher;
        private readonly DispatcherPriority _priority;

        public DispatcherDelegateCommand(
            Dispatcher dispatcher, Action command, bool canExecute = true, 
            DispatcherPriority priority = DispatcherPriority.Normal) 
            : base(command, canExecute)
        {
            _dispatcher = dispatcher;
            _priority = priority;
        }

        public DispatcherDelegateCommand(
            Dispatcher dispatcher, Action<object> command, bool canExecute = true, 
            DispatcherPriority priority = DispatcherPriority.Normal)
            : base(command, canExecute)
        {
            _dispatcher = dispatcher;
            _priority = priority;
        }

        public override void Execute(object parameter)
        {
            _dispatcher.BeginInvoke(new Action(() => base.Execute(parameter)), _priority);
        }
    }

    public class AsyncDelegateCommand : DelegateCommand
    {
        public AsyncDelegateCommand(Action command, bool canExecute = true) : base(command, canExecute)
        {
        }

        public AsyncDelegateCommand(Action<object> command, bool canExecute = true) : base(command, canExecute)
        {
        }

        public override void Execute(object parameter)
        {
            new Action(() => base.Execute(parameter)).BeginInvoke(null, null);
        }
    }
}