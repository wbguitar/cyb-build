using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskLib;

namespace CYB_Build.ViewModel.Commands
{
    public class EditCommand : ACommand
    {
        public EditCommand()
        {
            ConfigsVM.Instance.PropertyChanged += Instance_PropertyChanged;

            TaskRunner.Instance.OnStart += () => RaiseCanExecuteChanged();
            TaskRunner.Instance.OnEnd += (ec) => RaiseCanExecuteChanged();

            var cancel = true;
            //App.Current.MainWindow.Closing += (s, e) =>
            //{
            //    cancel = false;
            //    if (new System.Windows.Interop.WindowInteropHelper(cw).Handle != IntPtr.Zero)
            //        cw.Close();
            //};

            cw.Closing += (s, e) =>
            {
                cw.Visibility = Visibility.Hidden;
                e.Cancel = cancel;
            };

        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConfigsVM.Instance.SelectedConfig))
                RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return ConfigsVM.Instance.SelectedConfig != null;// && ConfigsVM.Instance.SelectedConfig.Status != TaskStatus.Running; // && !TaskRunner.Instance.Running;
        }

        ConfigWindow cw = new ConfigWindow();
        public override void Execute(object parameter)
        {
            //var cw = new ConfigWindow();
            ConfigVM.Instance.Config = ConfigsVM.Instance.SelectedConfig;
            var i = ConfigsVM.Instance.Configs.IndexOf(ConfigsVM.Instance.SelectedConfig);

            // se esiste uso un controllo custom
            var customCtrl = ConfigVM.Instance.GetCustomControl(ConfigVM.Instance.Config);
            if (customCtrl != null)
                cw.Content = customCtrl;

            cw.ShowDialog();

            ConfigsVM.Instance.Configs[i] = ConfigsVM.Instance.SelectedConfig = ConfigVM.Instance.Config;
            ConfigsVM.Instance.RefreshConfigs();
        }
    }
}
