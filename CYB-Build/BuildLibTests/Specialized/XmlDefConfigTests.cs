using NUnit.Framework;
using TaskLib.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TaskLib.Utils;

namespace TaskLib.Specialized.Tests
{
    [TestFixture()]
    public class XmlDefConfigTests
    {
        XmlDefConfig Create()
        {
            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
            var configFile = Path.Combine(currentDirectory, "xmldefconfig.xml");
            var cfg = new XmlDefConfig();
            cfg.Load(configFile);

            return cfg;
        }

        [Test()]
        public void LoadTest()
        {
            var cfg = Create();
            cfg.Validate();
        }
        
        [Test()]
        public void CreateTest()
        {
            var cfg = Create();
            var task = cfg.Create();
            Assert.AreEqual(task.GetType(), typeof(XmlDefTask), $"Wrong task type: {task.GetType().FullName}");
        }

        [Test()]
        public void ValidateTest()
        {
            Create().Validate();
        }

        [Test()]
        public void ReadXmlTest()
        {
            var cfg = new XmlDefConfig();
            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
            var configFile = Path.Combine(currentDirectory, "xmldefconfig.xml");
            cfg = cfg.Load(configFile);
        }

        [Test()]
        public void WriteXmlTest()
        {
            var fname = Path.GetTempFileName();
            Create().Save(fname);
            var xml = File.ReadAllText(fname);
            Console.WriteLine(xml);
        }
    }
}