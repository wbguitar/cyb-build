using TaskLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using STT = System.Threading.Tasks;
using System.Diagnostics;

namespace TaskLib
{
    public partial class TaskRunner : List<ITask>, ITask
    {
        private static Lazy<TaskRunner> lazy = new Lazy<TaskRunner>(() => new TaskRunner());
        public static TaskRunner Instance { get; } = lazy.Value;

        public string Name => "ProcessRunner";

        private TaskRunner()
        {
            OnStart += () => Running = true;
            OnEnd += (ec) => Running = false;
        }

        public bool Running { get; private set; }

        public event Action OnStart = () => { };
        public event Action<int> OnEnd = (ec) => { };

        public event Action<ITask> OnBeforeRun = (p) => { };
        public event Action<ITask, int> OnSuccess = (p, ec) => { };
        public event Action<ITask, int> OnFail = (p, ec) => { };

        public event Action<ITask, Process, string> OnStdErr = (ip, p, s) => { };
        public event Action<ITask, Process, string> OnStdOut = (ip, p, s) => { };
        
        public int Run()
        {
            OnStart();
            foreach (var proc in this)
            {
                int ecode = 0;
                OnBeforeRun(proc);
                try
                {
                    ecode = proc.Run();
                }
                catch (Exception exc)
                {
                    OnStdErr(proc, null, $"Exception running process {Name}: {exc.ToString()}");
                    ecode = -1;
                }
                
                if (ecode != 0)
                {
                    OnFail(proc, ecode);
                    OnEnd(ecode);
                    return ecode;
                }

                OnSuccess(proc, ecode);
            }

            OnEnd(0);
            return 0;
        }

        public async STT.Task<int> RunAsync()
        {
            return await STT.Task.Run<int>(() => Run());
        }
        
        /// <summary>
        /// Creates a <see cref="Task"/> object with the config passed and adds to the list
        /// </summary>
        /// <param name="cfg"></param>
        public ITask Setup(AConfig cfg)
        {
            var task = new Task();
            task.Setup(cfg);

            //task.OnStdErr += (ip, p, e) => OnStdErr(ip, p, e);
            //task.OnStdOut += (ip, p, e) => OnStdOut(ip, p, e);

            //base.Add(task);
            this.Add(task);

            PersistanceMgr.Instance.Configs.Cfgs.Add(cfg);

            return task;
        }

        public new void Add(ITask task)
        {
            task.OnStdErr += (ip, p, e) => OnStdErr(ip, p, e);
            task.OnStdOut += (ip, p, e) => OnStdOut(ip, p, e);

            base.Add(task);
        }

    }
}
