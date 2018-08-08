using NUnit.Framework;
using TaskLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLib.Tests
{
    [TestFixture()]
    public class TaskTests
    {
        [Test()]
        public void SetupTest()
        {
            Task proc = new Task();
            var cfg = new PingCfg();
            proc.Setup(cfg);

            try
            {
                cfg.Args = "dfsa";
                proc.Setup(cfg);
            }
            catch (Exception e)
            {
                var eType = typeof(InvalidConfigException);
                Assert.That(e.GetType() == eType, $"Exception should be of tipe {eType.Name}, cautght exception of type {e.GetType().Name}");
            }
        }

        [Test()]
        public void RunTest()
        {
            Task proc = new Task();
            var cfg = new PingCfg();
            proc.Setup(cfg);

            proc.OnStdErr += (ip, p, msg) => Console.WriteLine($"{ip.Name} Error - {msg}");
            proc.OnStdOut += (ip, p, msg) => Console.WriteLine($"{ip.Name} Out - {msg}");

            var exitCode = proc.Run();
            Assert.That(exitCode == 0, "Exit code should be 0");

            var cfg1 = new IpconfigCfg();
            proc.Setup(cfg1);
            exitCode = proc.Run();
            Assert.That(exitCode == 0, "Exit code should be 0");

            cfg1.Args = "fdsa";
            proc.Setup(cfg1);
            exitCode = proc.Run();
            Assert.That(exitCode != 0, "Exit code should not be 0");
        }
    }
}