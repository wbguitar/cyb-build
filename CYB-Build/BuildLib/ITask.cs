using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLib
{
    public interface ITask
    {
        string Name { get; }
        int Run();
        ITask Setup(AConfig cfg);

        event Action<ITask, Process, string> OnStdErr;
        event Action<ITask, Process, string> OnStdOut;
    }
}
