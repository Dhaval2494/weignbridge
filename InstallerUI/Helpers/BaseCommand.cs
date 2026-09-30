using System;
using System.Windows.Input;

namespace InstallerUI.Helpers
{
    public class BaseCommand : ICommand
    {
        private readonly Action<object> ExecuteAction;
        private readonly Func<object, bool> CanExecuteAction;

        public BaseCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
        {
            ExecuteAction = executeAction;
            CanExecuteAction = canExecuteAction;
        }

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteAction(parameter);
        }

#if !SILVERLIGHT
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
#else
        public event EventHandler CanExecuteChanged;
#endif

        #endregion 
    }
}