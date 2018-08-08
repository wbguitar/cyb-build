using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYB_Build.ViewModel.Commands
{
    public class SaveCommand : ExportCommand
    {
        public SaveCommand()
        {
            ConfigsVM.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CurrentFile")
                    RaiseCanExecuteChanged();
            };
        }

        public override bool CanExecute(object parameter)
        {
            return File.Exists(ConfigsVM.Instance.CurrentFile);
        }

        public override void Execute(object parameter)
        {
            DoSave(ConfigsVM.Instance.CurrentFile);
        }
    }
}
