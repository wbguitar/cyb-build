using Catel.Logging;
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

namespace CYB_Build.View
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
            lv.UnderlyingLogViewerControl.EnableTimestamp = true;
        }

        private void lv_Loaded(object sender, RoutedEventArgs e)
        {
            //var logger = LogVM.Instance.Logger;
            //logger.LogInfoHeading("INFO HEAD", "test");
            //logger.LogInfoHeading1("INFO HEAD 1", "test");
            //logger.LogInfoHeading2("INFO HEAD 2", "test");
            //logger.LogInfoHeading3("INFO HEAD 3", "test");
            //logger.Info("diocane");

            //logger.Error("PORCO DIO");
            //logger.Warning("UAUAUAU");
            //logger.Debug("DBG");
        }
    }
}
