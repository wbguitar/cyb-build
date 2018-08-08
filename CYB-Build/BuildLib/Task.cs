using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading;

namespace TaskLib
{
    public class Task : ITask
    {
        protected ProcessStartInfo psi;
        protected Process proc;

        public Process Process => proc;

        public event Action<ITask, Process, string> OnStdErr = (ip, p, s) => { };
        public event Action<ITask, Process, string> OnStdOut = (ip, p, s) => { };

        protected void raiseStdErr(ITask task, Process p, string msg)
        {
            OnStdErr(task, p, msg);
        }

        protected void raiseStdOut(ITask task, Process p, string msg)
        {
            OnStdOut(task, p, msg);
        }

        public string Name { get; protected set; }

        public virtual ITask Setup(AConfig cfg)
        {
            cfg.Validate();

            Name = cfg.Id;

            psi = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = cfg.Command,
                Arguments = cfg.Args,

                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            if (proc != null)
                proc.Close();

            proc = new Process()
            {
                StartInfo = psi,
            };

            return this;
        }

        public virtual int Run()
        {
            if (proc == null)
                return -1;

            proc.ErrorDataReceived += (o, e) => { if (e.Data != null) OnStdErr(this, o as Process, e.Data); };
            proc.OutputDataReceived += (o, e) => { if (e.Data != null) OnStdOut(this, o as Process, e.Data); };

            try
            {
                proc.Start();

                //if (proc.HasExited)
                //    throw new Exception($"Process {Name} has already been run");

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                proc.WaitForExit(Timeout.Infinite);
            }
            catch (Exception exc)
            {
                OnStdErr(this, proc, $"Error running process {this.Name}: {exc.ToString()}");
                return -1;
            }

            return proc.ExitCode;
        }

        protected void RaiseOnErr(Process p, string msg) => OnStdErr(this, p, msg);

        protected void RaiseOnOut(Process p, string msg) => OnStdOut(this, p, msg);
    }
}
