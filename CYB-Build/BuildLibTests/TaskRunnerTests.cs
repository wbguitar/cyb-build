using NUnit.Framework;
using NF = NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TaskLib.Utils;
using System.Diagnostics;

namespace TaskLib.Tests
{
    [TestFixture()]
    public class TaskRunnerTests
    {
        [Test()]
        public void RunTest()
        {
            var pr = TaskRunner.Instance;

            var pingCfg = new PingCfg();
            var ipconfigCfg = new IpconfigCfg();

            pr.OnSuccess += (p, ec) =>
            {
                Console.WriteLine($"process {p.Name} success");
                Assert.That(ec == 0, "Exit code must be 0");
            };

            pr.OnFail += (p, ec) =>
            {
                Console.WriteLine($"process {p.Name} failed");
                Assert.That(ec != 0, "Exit code must NOT be 0");
            };

            pr.OnStdErr += (ip, p, msg) => Console.WriteLine($"{ip.Name} Error - {msg}");
            pr.OnStdOut += (ip, p, msg) => Console.WriteLine($"{ip.Name} Out - {msg}");

            pr.Clear();

            //pr.Setup(pingCfg);
            //pr.Setup(ipconfigCfg);
            //pr.Setup(new MSBuildCfg());

            //var vbCfg = new VBBuildCfg();
            //var vbProc = pr.Setup(vbCfg);
            //pr.OnSuccess += (p, ec) =>
            //{
            //    if (vbProc == p)
            //    {
            //        var s = new StreamReader(File.OpenRead(VBBuildCfg.StdOutFile)).ReadToEnd();
            //        Console.WriteLine(s);
            //    }
            //};

            var tempPath = Path.GetTempPath();
            var tempDir = Path.Combine(tempPath, Path.GetRandomFileName());
            //Console.WriteLine($"Output directory: {tempDir}");
            Process.Start("explorer", $"{tempDir}");

            // GIT clone C#
            var gitcloneCSConfig = new Specialized.GitCloneTask.Config()
            {
                Command = @"git.exe",
                Args = $@"clone http://itmar-pc280:8442/r/CYB-CS.git --branch hotfix/M-P17042 {Path.Combine(tempDir, "CYB-CS")}",
                Id = "GIT clone C#",
            };
            Assert.AreEqual(gitcloneCSConfig.OutputDir, Path.Combine(tempDir, "CYB-CS"));
            Assert.AreEqual(gitcloneCSConfig.RepoUrl, "http://itmar-pc280:8442/r/CYB-CS.git");
            Assert.AreEqual(gitcloneCSConfig.Branch, "hotfix/M-P17042");
            var gitcloneCSTask = gitcloneCSConfig.Create();
            gitcloneCSTask.OnStdErr += (ip, p, msg) => Console.WriteLine($"{ip.Name} Error - {msg}");
            gitcloneCSTask.OnStdOut += (ip, p, msg) => Console.WriteLine($"{ip.Name} Out - {msg}");
            pr.Add(gitcloneCSTask);

            // GIT clone C# Libs
            var gitcloneCSLIBSConfig = new Specialized.GitCloneTask.Config()
            {
                Command = @"git.exe",
                Args = $@"clone http://itmar-pc280:8442/r/CYB-CS-LIBS.git --branch master ""{Path.Combine(tempDir, "Additional library")}""",
                Id = "GIT clone C#",
            };
            Assert.AreEqual(gitcloneCSLIBSConfig.OutputDir, $@"{Path.Combine(tempDir, "Additional library")}");
            Assert.AreEqual(gitcloneCSLIBSConfig.RepoUrl, "http://itmar-pc280:8442/r/CYB-CS-LIBS.git");
            Assert.AreEqual(gitcloneCSLIBSConfig.Branch, "master");
            var gitcloneCSLIBSTask = gitcloneCSLIBSConfig.Create();
            gitcloneCSLIBSTask.OnStdErr += (ip, p, msg) => Console.WriteLine($"{ip.Name} Error - {msg}");
            gitcloneCSLIBSTask.OnStdOut += (ip, p, msg) => Console.WriteLine($"{ip.Name} Out - {msg}");
            pr.Add(gitcloneCSLIBSTask);

            // MSBUILD
            var msbuildCfg = new Specialized.MSBuildTask.Config()
            {
                Command = @"msbuild.exe",
                //Args = $@"E:\dev\marini\m-p17022\UTS170022F001_SourceC#.9.6.23.4\CybertronicV9.6\Cybertronic\CybertronicSln.sln /p:Configuration=Release /p:OutputPath=E:\dev\marini\m-p17022\OUTPUTPATH",
                Args = $@"""{Path.Combine(gitcloneCSConfig.OutputDir, Specialized.Defaults.CSSln)}"" /p:Configuration=Release /p:OutputPath=""{Path.Combine(gitcloneCSConfig.OutputDir, "BUILD")}""",
                Id = "MSBuild task",
            };

            Assert.AreEqual(msbuildCfg.SolutionPath, gitcloneCSConfig.OutputDir);
            Assert.AreEqual(msbuildCfg.Configuration, Specialized.MSBuildTask.BuildConfig.Release);
            Assert.AreEqual(msbuildCfg.OutputDir, Path.Combine(gitcloneCSConfig.OutputDir, "BUILD"));

            var msbuildProc = msbuildCfg.Create();
            msbuildProc.OnStdErr += (ip, p, msg) => Console.WriteLine($"{ip.Name} Error - {msg}");
            msbuildProc.OnStdOut += (ip, p, msg) => Console.WriteLine($"{ip.Name} Out - {msg}");
            Assert.AreEqual(typeof(Specialized.MSBuildTask), msbuildProc.GetType(), $"Task type should be {msbuildProc.GetType()}");
            pr.Add(msbuildProc);

            // GIT clone VB
            var gitcloneVBConfig = new Specialized.GitCloneTask.Config()
            {
                Command = @"git.exe",
                Args = $@"clone http://itmar-pc280:8442/r/CYB-VB.git --branch develop ""{Path.Combine(tempDir, "CYB-VB")}""",
                Id = "GIT clone VB",
            };

            Assert.AreEqual(gitcloneVBConfig.OutputDir, Path.Combine(tempDir, "CYB-VB"));
            Assert.AreEqual(gitcloneVBConfig.RepoUrl, "http://itmar-pc280:8442/r/CYB-VB.git");
            Assert.AreEqual(gitcloneVBConfig.Branch, "develop");

            var gitcloneVBTask = gitcloneVBConfig.Create();
            gitcloneVBTask.OnStdErr += (ip, p, msg) => Console.WriteLine($"{ip.Name} Error - {msg}");
            gitcloneVBTask.OnStdOut += (ip, p, msg) => Console.WriteLine($"{ip.Name} Out - {msg}");
            pr.Add(gitcloneVBTask);

            // VB6
            var vbCfg = new Specialized.VBTask.Config()
            {
                Command = @"""C:\Program Files (x86)\Microsoft Visual Studio\VB98\VB6.EXE""",
                Args = $@"/make ""{Path.Combine(gitcloneVBConfig.OutputDir, Specialized.Defaults.VBProj)}"" /outdir ""{Path.Combine(gitcloneVBConfig.OutputDir, "BUILD")}""",
                Id = "VB6Build task",
            };

            Assert.AreEqual(vbCfg.OutDir, Path.Combine(gitcloneVBConfig.OutputDir, "BUILD"));
            Assert.AreEqual(vbCfg.ProjectPath, gitcloneVBConfig.OutputDir);

            var vbProc = vbCfg.Create(); // new Specialized.VBProc();
            Assert.AreEqual(typeof(Specialized.VBTask), vbProc.GetType(), $"Task type should be {vbProc.GetType()}");
            //vbProc.Setup(vbCfg);
            vbProc.OnStdErr += (ip, p, msg) => Console.WriteLine($"{ip.Name} Error - {msg}");
            vbProc.OnStdOut += (ip, p, msg) => Console.WriteLine($"{ip.Name} Out - {msg}");
            pr.Add(vbProc);

            // TELOCK
            //pr.Setup(new TElockCfg());
            var telockCfg = new Specialized.TELockTask.Config()
            {
                Command = @"C:\Users\btifac\Desktop\telock98\telock.exe",
                Args = $@"-S {Path.Combine(Path.Combine(gitcloneVBConfig.OutputDir, "BUILD"), Specialized.Defaults.CybExe)}",
                Id = "TELock task",
            };
            var telockProc = telockCfg.Create();

            Assert.AreEqual(typeof(Specialized.TELockTask), telockProc.GetType(), $"Task type should be {telockProc.GetType()}");
            pr.Add(telockProc);


            // IPConfig wrong
            ipconfigCfg.Args = "fdsa";
            pr.Setup(ipconfigCfg);


            // RUN
            pr.Run();
        }

        [Test()]
        public void SetupTest()
        {
            var pr = TaskRunner.Instance;
            var pingCfg = new PingCfg();
            var ipconfigCfg = new IpconfigCfg();

            pr.Clear();
            pr.Setup(pingCfg);
            pr.Setup(ipconfigCfg);
        }
    }
}