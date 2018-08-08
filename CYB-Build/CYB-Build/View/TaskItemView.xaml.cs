using CYB_Build.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CYB_Build.ViewModel;

namespace CYB_Build.View
{
    /// <summary>
    /// Interaction logic for TaskItemView.xaml
    /// </summary>
    public partial class TaskItemView : UserControl, INotifyPropertyChanged
    {
        public TaskItemView()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        public TaskItem Item
        {
            get => (TaskItem)GetValue(ItemProperty);
            set { SetValue(ItemProperty, value); PropertyChanged(this, new PropertyChangedEventArgs(nameof(Item))); }
        }

        // Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(TaskItem), typeof(TaskItemView), new PropertyMetadata(null));

    }
}
