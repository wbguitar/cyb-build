using TaskLib;
using TaskLib.Specialized;
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

namespace CYB_Build.View
{
    using WF = System.Windows.Forms;

    /// <summary>
    /// Interaction logic for ConfigCtrl.xaml
    /// </summary>
    public partial class ConfigCtrl : UserControl, INotifyPropertyChanged
    {

        public AConfig Config { get; set; }

        public ConfigCtrl()
        {
            InitializeComponent();
            
        }

        private void comboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var configType = comboTypes.SelectedItem as Type;
            var ctor = configType.GetConstructor(new Type[] { });
            var config = ctor.Invoke(new object[] { }) as AConfig;

            grid.SelectedObject = config;
        }

        WF.PropertyGrid grid = new WF.PropertyGrid();

        public event PropertyChangedEventHandler PropertyChanged = (s,e) => { };
        void onPropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        ObservableCollection<Type> types;
        public ObservableCollection<Type> Types
        {
            get { return types; }
            set { types = value; onPropertyChanged("Types"); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            wfHost.Child = grid;
            grid.PropertyValueChanged += (s, ea) => Config = Config;

            Types = new ObservableCollection<Type>
            {
                typeof(MSBuildTask.Config),
                typeof(VBTask.Config),
                typeof(GitCloneTask.Config),
                typeof(TELockTask.Config),
            };

        }
    }
}
