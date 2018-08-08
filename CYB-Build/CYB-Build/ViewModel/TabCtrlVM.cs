using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using TaskLib.Utils;

namespace CYB_Build.ViewModel
{
    public class TabCtrlVM : ViewModel<TabCtrlVM>
    {
        public static readonly string DefaultHeader = "TabItem";

        private TabItem selectedItem;
        public TabItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                RaisePropertyChanged(nameof(SelectedItem));
            }
        }

        private ObservableCollection<TabItem> items = new ObservableCollection<TabItem>();
        public ObservableCollection<TabItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is TabItem)
                return true;

            // TODO: altri comandi?

            return true;
        }

        public override void Execute(object parameter)
        {
            if (parameter is TabItem)
            {
                if (Items.Count > 2)
                    Items.Remove(parameter as TabItem);
            }
        }

        public TabItem AddItem(string header = null)
        {
            if (header.IsNullOrEmpty())
                header = DefaultHeader;

            var item = new TabItem() { Header = header };
            Items.Insert(Items.Count - 1, item);
            return item;
        }
    }
}
