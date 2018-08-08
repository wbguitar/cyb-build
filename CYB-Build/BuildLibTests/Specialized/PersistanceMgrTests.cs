using NUnit.Framework;
using TaskLib.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;

namespace TaskLib.Specialized.Tests
{
    [TestFixture()]
    public class PersistanceMgrTests
    {
        public string GetResourceTextFile(string filename)
        {
            string result = string.Empty;

            using (Stream stream = this.GetType().Assembly.
                       GetManifestResourceStream("assembly.folder." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        [Test()]
        public void LoadTest()
        {
            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
            var configFile = Path.Combine(currentDirectory, "configs.xml");
            var loaded = TaskRunner.Instance.Load(configFile);
            Assert.IsTrue(loaded, $"Loading Configs failed");
        }

        [Test()]
        public void SaveTest()
        {
            var tempPath = Path.GetTempFileName();
            var tempDir = Path.GetDirectoryName(tempPath);
            Process.Start("explorer", $"{tempDir}");

            // GIT clone C#
            var gitcloneCSConfig = new Specialized.GitCloneTask.Config()
            {
                Command = @"git.exe",
                Args = $@"clone http://itmar-pc280:8442/r/CYB-CS.git --branch hotfix/M-P17042 {Path.Combine(tempDir, "CYB-CS")}",
                Id = "GIT clone C#",
            };
            // GIT clone C# Libs
            var gitcloneCSLIBSConfig = new Specialized.GitCloneTask.Config()
            {
                Command = @"git.exe",
                Args = $@"clone http://itmar-pc280:8442/r/CYB-CS-LIBS.git --branch master ""{Path.Combine(tempDir, "Additional library")}""",
                Id = "GIT clone C#",
            };

            // MSBUILD
            var msbuildCfg = new Specialized.MSBuildTask.Config()
            {
                Command = @"msbuild.exe",
                //Args = $@"E:\dev\marini\m-p17022\UTS170022F001_SourceC#.9.6.23.4\CybertronicV9.6\Cybertronic\CybertronicSln.sln /p:Configuration=Release /p:OutputPath=E:\dev\marini\m-p17022\OUTPUTPATH",
                Args = $@"""{Path.Combine(gitcloneCSConfig.OutputDir, Defaults.CSSln)}"" /p:Configuration=Release /p:OutputPath=""{Path.Combine(gitcloneCSConfig.OutputDir, "BUILD")}""",
                Id = "MSBuild task",
            };
            // GIT clone VB
            var gitcloneVBConfig = new Specialized.GitCloneTask.Config()
            {
                Command = @"git.exe",
                Args = $@"clone http://itmar-pc280:8442/r/CYB-VB.git --branch develop ""{Path.Combine(tempDir, "CYB-VB")}""",
                Id = "GIT clone VB",
            };
            // VB6
            var vbCfg = new Specialized.VBTask.Config()
            {
                Command = @"""C:\Program Files (x86)\Microsoft Visual Studio\VB98\VB6.EXE""",
                Args = $@"/make ""{Path.Combine(gitcloneVBConfig.OutputDir, Defaults.VBProj)}"" /outdir ""{Path.Combine(gitcloneVBConfig.OutputDir, "BUILD")}""",
                Id = "VB6Build task",
            };
            // TELOCK
            //pr.Setup(new TElockCfg());
            var telockCfg = new Specialized.TELockTask.Config()
            {
                Command = @"C:\Users\btifac\Desktop\telock98\telock.exe",
                Args = $@"-S {Path.Combine(Path.Combine(gitcloneVBConfig.OutputDir, "BUILD"), Defaults.CybExe)}",
                Id = "TELock task",
            };

            var cf = PersistanceMgr.Instance.Configs.Cfgs;
            cf.Add(gitcloneCSConfig);
            cf.Add(gitcloneCSLIBSConfig);
            cf.Add(msbuildCfg);
            cf.Add(gitcloneVBConfig);
            cf.Add(vbCfg);
            cf.Add(telockCfg);

            bool saved = PersistanceMgr.Instance.Save(tempPath);
            Assert.IsTrue(saved, $"Saving Configs failed");
            //XDocument.Load(tempPath);
           
        }
    }
}