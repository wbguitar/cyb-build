using Catel.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskLib;

namespace CYB_Build.ViewModel.Commands
{

    public class ExportCommand : ACommand
    {
        public ExportCommand()
        {
            ConfigsVM.Instance.PropertyChanged += Instance_PropertyChanged;

            TaskRunner.Instance.OnStart += () => RaiseCanExecuteChanged();
            TaskRunner.Instance.OnEnd += (ec) => RaiseCanExecuteChanged();
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Configs")
                RaiseCanExecuteChanged();
        }

        ~ExportCommand()
        {
            ConfigsVM.Instance.PropertyChanged -= Instance_PropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return ConfigsVM.Instance.Configs?.Count > 0 && !TaskRunner.Instance.Running;
        }

        public override void Execute(object parameter)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "XML File (*.xml)|*.xml",
            };
            if (!sfd.ShowDialog().Value)
                return;

            if (!DoSave(sfd.FileName))
                return;

            Utils.AddRecent(sfd.FileName);
        }

        public bool DoSave(string fname)
        {
            var configs = ConfigsVM.Instance.Configs;
            PersistanceMgr.Instance.Configs.Cfgs = new List<AConfig>(configs);
            PersistanceMgr.Instance.Configs.TaskProcessTitle = TaskProcessVM.Instance.SelectedTask.Title;
            var retval = PersistanceMgr.Instance.Save(fname);
            if (!retval)
                LogVM.Instance.Logger.Error($"Error saving to file {fname}");
            else
            {
                ConfigsVM.Instance.CurrentFile = fname;
                LogVM.Instance.Logger.Info($"Configuration exported to file {fname}");
            }

            return retval;
        }
    }
}
