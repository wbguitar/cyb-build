using TaskLib;
using TaskLib.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using CYB_Build.Model;

namespace CYB_Build.ViewModel
{
    public interface INotifyPropertyValueChanged
    {
        event PropertyValueChangedEventHandler PropertyValueChanged;
    }

    public class ConfigVM : ViewModel<ConfigVM>, INotifyPropertyValueChanged
    {
        public ConfigVM()
        {
            var asses = TaskProcessVM.Instance.Assemblies;

            var types = new List<Type>();
            foreach (var ass in asses)
            {
                types.AddRange(typeof(AConfig).EnumerateTypes(ass.Assembly));
            }

            Types = new ObservableCollection<Type>(types);
        }

        private AConfig config;
        public AConfig Config
        {
            get { return config; }
            set
            {
                config = value;
                RaisePropertyChanged("Config");
            }
        }

        ObservableCollection<Type> types;
        public ObservableCollection<Type> Types
        {
            get { return types; }
            set { types = value; RaisePropertyChanged("Types"); }
        }

        private Type selectedType;

        public Type SelectedType
        {
            get { return selectedType; }
            set { selectedType = value; RaisePropertyChanged("SelectedType"); }
        }

        public event PropertyValueChangedEventHandler PropertyValueChanged = (s, e) => { };

        bool recursiveCall = false;
        public void RaisePropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (recursiveCall)
                return;

            recursiveCall = true;
            PropertyValueChanged(sender, e);
            ConfigsVM.Instance.RefreshConfigs();
            recursiveCall = false;
        }

        Dictionary<Type, Type> controlsTable = new Dictionary<Type, Type>();
        public void RegisterControl(Type configType, Type controlType)
        {
            if (!configType.IsSubclassOf(typeof(AConfig)))
                throw new InvalidOperationException($"Invalid config type ({configType.FullName}), AConfig subclass expected");

            if (!controlType.IsSubclassOf(typeof(UserControl)))
                throw new InvalidOperationException($"Invalid control type ({configType.FullName}), UserControl subclass expected");

            controlsTable[configType] = controlType;
        }

        public UserControl GetCustomControl(AConfig cfg)
        {
            if (!controlsTable.ContainsKey(cfg.GetType()))
                return null;

            return controlsTable[cfg.GetType()]
                .GetConstructor(new Type[] { })
                .Invoke(null) as UserControl;
        }
    }
}
