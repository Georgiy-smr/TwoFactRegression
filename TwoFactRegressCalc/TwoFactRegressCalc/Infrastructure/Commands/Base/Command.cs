using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TwoFactRegressCalc.Infrastructure.Commands.Base
{
    public abstract class Command : ICommand
    {
        #region ICommand 

        private event EventHandler CanExecuteChangedHandlers;
        protected virtual void OnCanExecuteChanged(EventArgs e = null)
            => CanExecuteChangedHandlers?.Invoke(this, e ?? EventArgs.Empty);
        /// <summary>Событие возникает при изменении возможности исполнения команды</summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedHandlers += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedHandlers -= value;
            }
        }

        #endregion

        private bool _Executable = true;

        bool ICommand.CanExecute(object parameter) => CanExecute(parameter) && _Executable;

        void ICommand.Execute(object parameter)
        {
            if (CanExecute(parameter))
                Execute(parameter);
        }

        protected virtual bool CanExecute(object parameter) => true;
        protected abstract void Execute(object parameter);
    }
}
