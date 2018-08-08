using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CYB_Build.ViewModel
{
    [Serializable]
    public abstract class ANotifyPropertyChanged : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        protected void RaisePropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class ViewModel<T> : ANotifyPropertyChanged, ICommand
        where T : new()
    {
        public static event Action ValueCreated = () => { };

        //protected static Lazy<T> inst = new Lazy<T>(() => new T());
        //public static T Instance
        //{
        //    get
        //    {
        //        var val = inst.Value;
        //        ValueCreated();
        //        return val;
        //    }
        //}

        protected static T _inst;
        public static T Instance
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new T();
                    ValueCreated();
                }

                return _inst;
            }
        }

        public event EventHandler CanExecuteChanged = (s, e) => { };

        public virtual bool CanExecute(object parameter)
        {
            return false;
        }

        public virtual void Execute(object parameter)
        {
        }

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        protected void RaiseCanExecuteChanged(object sender, EventArgs ea)
        {
            CanExecuteChanged(sender, ea);
        }
    }

    public class Singleton<T> where T : new()
    {
        protected static Lazy<T> inst = new Lazy<T>(() => new T());
        public static T Instance { get { return inst.Value; } }
    }
}
