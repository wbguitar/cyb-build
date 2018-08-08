using CYB_Build.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskLib;

namespace CYB_Build.ViewModel.Commands
{
    public class NewBuildCommand : ACommand
    {
        public NewBuildCommand()
        {
            TaskRunner.Instance.OnStart += () => RaiseCanExecuteChanged();
            TaskRunner.Instance.OnEnd += (ec) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return !TaskRunner.Instance.Running;
        }

        BindingManager bm;
        public override void Execute(object parameter)
        {
            //var tp = new TaskProcess();
            var title = (string)parameter;
            var tp = TaskProcessVM.Instance.TaskProcs.FirstOrDefault(t => t.Title == title);
            if (tp == null)
                return;

            //tp.LoadDefault();

            if (bm != null)
                bm.Dispose();

            bm = new BindingManager(tp);

            var procDef = tp.Generate();
            bm.SetupBindings(procDef);

            ConfigsVM.Instance.Configs = new System.Collections.ObjectModel.ObservableCollection<AConfig>(procDef.Keys);
        }
    }

}
