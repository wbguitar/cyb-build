using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskLib;

namespace CYB_Build.ViewModel.Commands
{

    public class RunCommand : ACommand
    {
        public RunCommand()
        {
            TaskRunner.Instance.OnStart += () => RaiseCanExecuteChanged();
            TaskRunner.Instance.OnEnd += (ec) => RaiseCanExecuteChanged();

            TaskRunner.Instance.OnSuccess += Instance_OnSuccess;
            TaskRunner.Instance.OnFail += Instance_OnFail;
            TaskRunner.Instance.OnBeforeRun += Instance_OnBeforeRun;
        }

        private void Instance_OnBeforeRun(ITask obj)
        {
            Invoke(new Action(() =>
            {
                var found = ConfigsVM.Instance.Configs.FirstOrDefault(cfg => cfg.Id == obj.Name);
                if (found != null)
                {
                    var idx = ConfigsVM.Instance.Configs.IndexOf(found);
                    var clone = found.Clone() as AConfig;
                    clone.Status = TaskLib.TaskStatus.Running;
                    ConfigsVM.Instance.Configs[idx] = clone;
                }
            }));
        }

        private void Instance_OnFail(ITask arg1, int arg2)
        {
            Invoke(new Action(() =>
            {
                var found = ConfigsVM.Instance.Configs.FirstOrDefault(cfg => cfg.Id == arg1.Name);
                if (found != null)
                {
                    var idx = ConfigsVM.Instance.Configs.IndexOf(found);
                    var clone = found.Clone() as AConfig;
                    clone.Status = TaskLib.TaskStatus.Error;
                    ConfigsVM.Instance.Configs[idx] = clone;
                }
            }));
        }

        private void Instance_OnSuccess(ITask arg1, int arg2)
        {
            Invoke(new Action(() =>
            {
                var found = ConfigsVM.Instance.Configs.FirstOrDefault(cfg => cfg.Id == arg1.Name);
                if (found != null)
                {
                    var idx = ConfigsVM.Instance.Configs.IndexOf(found);
                    var clone = found.Clone() as AConfig;
                    clone.Status = TaskLib.TaskStatus.Done;
                    ConfigsVM.Instance.Configs[idx] = clone;
                }
            }));
        }

        //void RaiseCanExecuteChanged()
        //{
        //    Application.Current.Dispatcher.BeginInvoke(new Action(() => CanExecuteChanged(this, EventArgs.Empty)));
        //}

        public override bool CanExecute(object parameter)
        {
            return !TaskRunner.Instance.Running;
        }

        public override async void Execute(object parameter)
        {
            var parm = (string)parameter;
            var configs = ConfigsVM.Instance.Configs.AsEnumerable();

            if (parm == "continue")
                configs = configs.Where(c => c.Status != TaskLib.TaskStatus.Done);
            else if (parm == "run")
            {
                configs = ConfigsVM.Instance.Configs =
                    new ObservableCollection<AConfig>(ConfigsVM.Instance.Configs.Select(c => { c.Status = TaskLib.TaskStatus.Todo; return c; }));
            }

            TaskRunner.Instance.Clear();
            foreach (var config in configs)
            {
                TaskRunner.Instance.Add(config.Create());
            }

            await TaskRunner.Instance.RunAsync();
        }
    }
}
