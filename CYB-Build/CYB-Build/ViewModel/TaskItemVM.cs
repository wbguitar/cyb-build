using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CYB_Build.Model;
using CYB_Build.View;
using TaskLib.Utils;

namespace CYB_Build.ViewModel
{
    public class TaskItemVM : ViewModel<TaskItemVM>
    {
        private TaskItem _item;

        public TaskItem SelectedItem
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged(nameof(SelectedItem)); RaiseCanExecuteChanged(); }
        }


        private BindItem selectedBinding;
        public BindItem SelectedBinding
        {
            get { return selectedBinding; }
            set { selectedBinding = value; RaisePropertyChanged(nameof(SelectedBinding)); RaiseCanExecuteChanged(); }
        }

        public ObservableCollection<Type> Types
        {
            get { return ConfigVM.Instance.Types; }
        }


        public override void Execute(object parameter)
        {
            var cmd = (string)parameter;

            if (cmd == "AddBinding")
            {
                var bi = EditBinding(new BindItem(), EditMode.Add);
                if (bi == null)
                    return;

                SelectedItem.Bindings.Add(bi);
            }
            else if (cmd == "EditBinding")
            {
                var idx = SelectedItem.Bindings.IndexOf(SelectedBinding);
                var bi = EditBinding(SelectedBinding.Clone(), EditMode.Edit);
                if (bi == null)
                    return;

                SelectedItem.Bindings[idx] = SelectedBinding = bi;
            }
            else if (cmd == "RemoveBinding")
            {
                SelectedItem.Bindings.Remove(SelectedBinding);
            }

            SelectedItem.Bindings = new Bindings(SelectedItem.Bindings.AsEnumerable());
            SelectedItem = SelectedItem.Clone();
        }

        public override bool CanExecute(object parameter)
        {
            if (SelectedItem == null)
                return false;

            var cmd = (string)parameter;
            if (cmd == "AddBinding")
                return true;
            else if (cmd == "RemoveBinding" || cmd == "EditBinding")
                return SelectedBinding != null;
            return false;
        }

        BindItem EditBinding(BindItem bi, EditMode mode)
        {
            var w = new BindingItemView()
            {
                Mode = mode,
                TaskItem = this.SelectedItem,
                TaskProcess = TaskProcessVM.Instance.SelectedTask,
                BindItem = bi,
            };

            w.ShowDialog();
            if (w.DialogResult.Value)
                return w.BindItem;

            return null;
        }

    }
}
