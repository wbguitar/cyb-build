using CYB_Build.ViewModel;
using System;
using System.Collections.Generic;
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
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace CYB_Build.View
{
    /// <summary>
    /// Interaction logic for FinalTaskConfigView.xaml
    /// </summary>
    public partial class FinalTaskConfigView : UserControl, ITypeEditor
    {
        public FinalTaskConfigView()
        {
            InitializeComponent();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var arr = e.Changes.ToArray();
            var tb = sender as Xceed.Wpf.Toolkit.PropertyGrid.Editors.PropertyGridEditorTextBox;

            ConfigVM.Instance.RaisePropertyValueChanged(ConfigVM.Instance.Config, new PropertyValueChangedEventArgs(e.RoutedEvent, e.OriginalSource, null, tb.Text));
        }
    }
}
