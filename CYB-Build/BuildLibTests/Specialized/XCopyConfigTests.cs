using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaskLib.Specialized.XCopyTask;

namespace TaskLib.Specialized.Tests
{
    [TestFixture()]
    public class XCopyConfigTests
    {
        [Test()]
        public void CreateTest()
        {
            var tpath = Path.GetTempPath();
            var from = Directory.CreateDirectory(Path.Combine(tpath, Path.GetRandomFileName())).FullName;
            var to = Directory.CreateDirectory(Path.Combine(tpath, Path.GetRandomFileName())).FullName;

            var srcFiles = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                using (var f = File.Create(Path.Combine(from, Path.GetRandomFileName())))
                {
                    srcFiles.Add(Path.GetFileName(f.Name));
                }

            }

            var cfg = new XCopyConfig
            {
                Id = "copy task",
                Args = $@"""{from}"" ""{to}"" {XCopyConfig.Options}",
            };

            Assert.AreEqual(cfg.SourceDir, from);
            Assert.AreEqual(cfg.OutputDir, to);

            var task = cfg.Create();
            var ecode = task.Run();

            Assert.AreEqual(ecode, 0, "Return code should be 0");

            var destFiles = Directory.EnumerateFiles(to).Select(f => Path.GetFileName(f));

            Assert.IsFalse(srcFiles.Any(f => !destFiles.Contains(f)));
        }

    }
}