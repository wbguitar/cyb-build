using CYB_Build.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLib.Specialized.Tests
{
    [TestFixture()]
    public class EnvelopeConfigTests
    {
        [Test()]
        public void EnvelopeConfigTest()
        {
            //var bins = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            //var envsrc = @"E:\fake envelope\Non cript"; // Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            //var envdst = @"E:\fake envelope\Cript"; // Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            //var envset = Path.GetRandomFileName();
            //var cript = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            //var env = new EnvelopeTask.EnvelopeConfig()
            //{
            //    BinSources = bins,
            //    EnvSrcFolder = envsrc,
            //    EnvDestFolder = envdst,
            //    //EnvelopeSettings = envset,
            //    EnvelopeCommand = @"E:\fake envelope\envelope.bat",
            //    CriptedFolder = cript,
            //    Id = "test envelope",
            //};

            //Console.WriteLine(env.Command);
            //foreach (var cfg in env.Configs)
            //{
            //    Console.WriteLine($"{cfg.Command} {cfg.Args}");
            //}
            ////env.Validate();
            //var task = env.Create();
            //task.OnStdOut += Task_OnStdOut;
            //task.OnStdErr += Task_OnStdErr;
            //var ecode = task.Run();
            //Assert.AreEqual(ecode, 0);
        }

        private void Task_OnStdErr(ITask arg1, System.Diagnostics.Process arg2, string arg3)
        {
            Console.WriteLine($"ERR *** {arg3}");
        }

        private void Task_OnStdOut(ITask arg1, System.Diagnostics.Process arg2, string arg3)
        {
            Console.WriteLine($"{arg3}");
        }
    }
}