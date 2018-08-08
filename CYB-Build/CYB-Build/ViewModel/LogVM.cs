using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Logging;
using TaskLib;

namespace CYB_Build.ViewModel
{
    public class LogVM : ViewModel<LogVM>
    {
        public ILog Logger { get; set; } = LogManager.GetCurrentClassLogger();

        public LogVM()
        {
            TaskRunner.Instance.OnStart += Instance_OnStart;
            TaskRunner.Instance.OnEnd += Instance_OnEnd;
            TaskRunner.Instance.OnBeforeRun += Instance_OnBeforeRun;

            TaskRunner.Instance.OnSuccess += Instance_OnSuccess;
            TaskRunner.Instance.OnFail += Instance_OnFail;

            TaskRunner.Instance.OnStdErr += Instance_OnErr;
            TaskRunner.Instance.OnStdOut += Instance_OnOut;
        }

        private void Instance_OnEnd(int ecode)
        {
            if (ecode == 0)
                Logger.LogInfoHeading2("PROCESS SUCCESSFULLY FINISHED");
            else
                Logger.LogErrorHeading2("PROCESS ENDED WITH ERRORS");
        }

        private void Instance_OnStart()
        {
            Logger.LogInfoHeading("STARTING NEW PROCESS", "");
        }

        private void Instance_OnBeforeRun(ITask obj)
        {
            Logger.Info("Starting task {0}", obj.Name);
            if (obj is TaskLib.Task)
            {
                var si = (obj as TaskLib.Task).Process.StartInfo;
                Logger.Info($"{si.FileName} {si.Arguments}");
            }
        }

        private void Instance_OnSuccess(ITask arg1, int arg2)
        {
            Logger.LogInfoHeading1("Successfully finished task {0}", arg1.Name);
        }

        private void Instance_OnOut(ITask arg1, System.Diagnostics.Process arg2, string arg3)
        {
            Logger.Info(arg3);
        }

        private void Instance_OnFail(ITask arg1, int arg2)
        {
            Logger.Error("Task {0} finished with errors, exit code {1}", arg1.Name, arg2);
        }

        private void Instance_OnErr(ITask arg1, System.Diagnostics.Process arg2, string arg3)
        {
            Logger.Error(arg3);
        }
    }
}
