using TaskLib;
using TaskLib.Specialized;
using TaskLib.Utils;
using CYB_Build.ViewModel;
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
using System.Globalization;

namespace CYB_Build.View
{
    /// <summary>
    /// Interaction logic for ConfigCtrlEx.xaml
    /// </summary>
    public partial class ConfigCtrlEx : UserControl
    {
        public ConfigCtrlEx()
        {
            InitializeComponent();
            ConfigVM.Instance.PropertyChanged += Instance_PropertyChanged;

            Unloaded += ConfigCtrlEx_Unloaded;
            
            //ConfigVM.Instance.SelectedType = ConfigVM.Instance.Types.First();
        }

        private void ConfigCtrlEx_Unloaded(object sender, RoutedEventArgs e)
        {
            ConfigVM.Instance.PropertyChanged -= Instance_PropertyChanged;
        }
        
        bool insidePropertyChanged = false;
        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (insidePropertyChanged)
                return;

            insidePropertyChanged = true;
            if (e.PropertyName == "SelectedType")
            {
                var ctor = ConfigVM.Instance.SelectedType?.GetConstructor(new Type[] { });
                var config = ctor?.Invoke(new object[] { }) as AConfig;

                ConfigVM.Instance.Config = config;
            }
            else if (e.PropertyName == "Config")
            {
                ConfigVM.Instance.SelectedType = ConfigVM.Instance.Config.GetType();
            }

            insidePropertyChanged = false;
        }

        private void wfHost_PropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {
            // necessario per aggiornare i campi del propertygrid
            ConfigVM.Instance.Config = ConfigVM.Instance.Config.Clone() as AConfig;
            ConfigVM.Instance.RaisePropertyValueChanged(ConfigVM.Instance.Config, e);
        }
    }

    public class TypeValueConverter : IValueConverter
    {
        public static TypeValueConverter Instance { get; set; } = new TypeValueConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as Type;
            if (type == null)
                return null;

            var descr = type.GetDescriptionAttribute();
            if (string.IsNullOrEmpty(descr))
                descr = type.Name;

            return descr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var descr = value as String;
            if (descr == null)
                return null;

            foreach (var type in typeof(AConfig).EnumerateTypes())
            {
                if (type.GetDescriptionAttribute() == descr)
                    return type;
            }

            return null;
        }
    }

    public class TypesValueConverter : IValueConverter
    {
        public static TypesValueConverter Instance { get; set; } = new TypesValueConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var types = value as IEnumerable<Type>;
            if (types == null)
                return null;

            var descriptions = new List<string>();
            foreach (var type in types)
            {
                var descr = type.GetDescriptionAttribute();
                if (string.IsNullOrEmpty(descr))
                    descr = type.Name;
                descriptions.Add(descr);
            }

            return descriptions;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var descr = value as String;
            if (descr == null)
                return null;

            foreach (var type in typeof(AConfig).EnumerateTypes())
            {
                if (type.GetDescriptionAttribute() == descr)
                    return type;
            }

            return null;
        }
    }
}
