using Catel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYB_Build.ViewModel.Commands
{
    public class ImportRecentCommand : ACommand
    {
        public event Action<string> OnExecute;
        public ImportRecentCommand(ImportCommand cmd)
        {
            OnExecute = (file) =>
            {
                if (!cmd.ImportFile(file))
                    LogVM.Instance.Logger.Info($"Importing failed");
                else
                    TabCtrlVM.Instance.SelectedItem.Header = file;
            };
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var file = (string)parameter;
            if (!File.Exists(file))
            {
                Utils.RemoveRecent(file);
                LogVM.Instance.Logger.WarningAndStatus($"File {file} not found, removing from recents");
            }
            else
            {
                LogVM.Instance.Logger.Info($"Importing file {file}");
                OnExecute(file);
                Utils.AddRecent(file);
                RaiseExecuted();
            }
        }
    }

}
