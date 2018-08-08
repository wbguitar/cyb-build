using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CYB_Build.ViewModel.Commands
{
    public abstract class ACommand : ICommand
    {
        public event EventHandler CanExecuteChanged = (s, e) => { };

        public event Action Executed = delegate { };
        protected void RaiseExecuted() => BeginInvoke(new Action(() => Executed()));

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);

        protected void RaiseCanExecuteChanged()
        {
            BeginInvoke(new Action(() => CanExecuteChanged(this, EventArgs.Empty)));
        }

        protected void RaiseCanExecuteChanged(object sender, EventArgs ea)
        {
            BeginInvoke(new Action(() => CanExecuteChanged(this, EventArgs.Empty)));
        }

        protected void BeginInvoke(Delegate d)
        {
            Application.Current.Dispatcher.BeginInvoke(d);
        }

        protected void Invoke(Delegate d)
        {
            Application.Current.Dispatcher.Invoke(d);
        }
    }
}
