using CYB_Build.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Input;
using System.Windows;
using CYB_Build.View;
using TaskLib.Utils;

namespace CYB_Build.ViewModel
{
    public class TaskDefsVM : ViewModel<TaskDefsVM>
    {
        private ObservableCollection<TaskItem> items;

        public ObservableCollection<TaskItem> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(nameof(Items)); }
        }

        private TaskItem selectedItem;
        public TaskItem SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; RaisePropertyChanged(nameof(SelectedItem)); RaiseCanExecuteChanged(); }
        }

        public override void Execute(object parameter)
        {
            var cmd = (string)parameter;
            if (cmd == "Add")
            {
                var ti = TaskItemVM.Instance.SelectedItem = new TaskItem();
                if (!ti.Equals(EditItem(ti, EditMode.Add)))
                    Items.Add(ti);
                else
                    TaskItemVM.Instance.SelectedItem = null;
            }
            else if (cmd == "Remove")
            {
                Items.Remove(SelectedItem);
            }

            if (SelectedItem != null)
            {
                SelectedItem.Bindings = new Bindings(SelectedItem.Bindings.AsEnumerable());
                SelectedItem = SelectedItem.Clone();
            }
        }

        public override bool CanExecute(object parameter)
        {
            var cmd = (string)parameter;
            if (cmd == "Add")
                return true;
            else if (cmd == "Remove")
                return SelectedItem != null;

            return false;
        }


        TaskItem EditItem(TaskItem ti, EditMode mode)
        {
            var tiView= new TaskItemView()
            {
                Item = ti.Clone(),
            };

            var w = new Window()
            {
                Width = 350,
                Height = 150,
                Content = tiView
            };
            
            w.ShowDialog();
            if (w.DialogResult.Value)
                return tiView.Item;

            return null;
        }
    }
}
