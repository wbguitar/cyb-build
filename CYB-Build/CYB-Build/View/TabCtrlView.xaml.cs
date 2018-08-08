using CYB_Build.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using TaskLib;
using SIO = System.IO;

namespace CYB_Build.View
{
    /// <summary>
    /// Interaction logic for TabCtrlView.xaml
    /// </summary>
    public partial class TabCtrlView : UserControl
    {
        public TabCtrlView()
        {
            InitializeComponent();

            var tbAdd = new TextBlock()
            {
                FontSize = 8,
                Margin = new Thickness(-2, 0, 0, 0),
                Text = "+",
                Background = Brushes.Transparent,
            };


            //var ti = new TabItem() { Header = tbAdd };
            var curFolder = SIO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://siteoforigin:,,,/Resources/plus.png"); // new Uri(SIO.Path.Combine(curFolder, "Resources/plus.png"))
            bi.EndInit();
            var ti = new TabItem()
            {
                Header = new Image()
                {
                    Source = bi,
                    Width = 18,
                    Margin = new Thickness(-5, -10, -5, -10),
                },
            };

            ti.Style = null;

            TabCtrlVM.Instance.Items.Add(ti);

            tabCtrl.SelectionChanged += (s, e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (Application.Current.MainWindow == null ||
                        !Application.Current.MainWindow.IsInitialized)
                        return;

                    if (e.AddedItems?.Count > 0 && e.AddedItems[0] == ti)
                    {
                        TabCtrlVM.Instance.SelectedItem = TabCtrlVM.Instance.AddItem();
                    }
                }));

            };



            TabCtrlVM.Instance.Items.CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null || e.NewItems.Count == 0)
                    return;

                (e.NewItems[0] as TabItem).Content = new ConfigsView()
                {
                    Tag = new ObservableCollection<AConfig>()
                };
            };

            ConfigsVM.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ConfigsVM.Instance.Configs))
                {
                    TabCtrlVM.Instance.SelectedItem.Tag = ConfigsVM.Instance.Configs;
                }
            };

            tabCtrl.SelectionChanged += (s, e) =>
                {
                    if (e.AddedItems?.Count == 0 || e.AddedItems[0] == ti)
                        return;

                    ConfigsVM.Instance.Configs = (TabCtrlVM.Instance.SelectedItem.Tag as ObservableCollection<AConfig>);
                };


            TabCtrlVM.Instance.AddItem();
        }

    }
}
