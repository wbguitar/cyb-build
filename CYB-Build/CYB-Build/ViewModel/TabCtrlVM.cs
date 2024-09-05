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
    public class TabItemEx : TabItem, INotifyPropertyChanged
    {

        private string processTitle;

        public string ProcessTitle
        {
            get { return processTitle; }
            set { processTitle = value; RaisePropertyChanged(nameof(ProcessTitle)); }
        }

        void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TabCtrlVM : ViewModel<TabCtrlVM>
    {
        public static readonly string DefaultHeader = "TabItem";

        private TabItemEx selectedItem;
        public TabItemEx SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                RaisePropertyChanged(nameof(SelectedItem));
            }
        }

        private ObservableCollection<TabItemEx> items = new ObservableCollection<TabItemEx>();
        public ObservableCollection<TabItemEx> Items
        {
            get { return items; }
            set { items = value; }
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is TabItemEx)
                return true;

            // TODO: altri comandi?

            return true;
        }

        public override void Execute(object parameter)
        {
            if (parameter is TabItem)
            {
                if (Items.Count > 2)
                    Items.Remove(parameter as TabItemEx);
            }
        }

        public TabItemEx AddItem(string header = null)
        {
            if (header.IsNullOrEmpty())
                header = DefaultHeader;

            var item = new TabItemEx() { Header = header };
            Items.Insert(Items.Count - 1, item);
            return item;
        }
    }
}
