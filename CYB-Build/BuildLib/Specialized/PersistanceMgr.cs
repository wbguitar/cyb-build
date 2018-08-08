using TaskLib.Specialized;
using TaskLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TaskLib
{
    [XmlRoot]
    [XmlInclude(typeof(MSBuildTask.Config))]
    [XmlInclude(typeof(GitCloneTask.Config))]
    [XmlInclude(typeof(TELockTask.Config))]
    [XmlInclude(typeof(VBTask.Config))]
    public class Configs
    {
        [XmlElement]
        public List<AConfig> Cfgs { get; set; } = new List<AConfig>();

        [XmlAttribute]
        public string TaskProcessTitle { get; set; }
    }

    public class PersistanceMgr : List<AConfig>
    {
        private static Lazy<PersistanceMgr> inst = new Lazy<PersistanceMgr>(() => new PersistanceMgr());
        public static PersistanceMgr Instance => inst.Value;


        public Configs Configs { get; private set; } = new Configs();

        private PersistanceMgr()
        { }

        public bool Load(string path)
        {
            var extratypes = typeof(AConfig).FindSubTypes().ToArray();
            Configs = Configs.Load(path, extratypes, new System.IO.StreamWriter(Console.OpenStandardOutput()));
            return true;
        }

        public bool Save(string path)
        {
            var extratypes = typeof(AConfig).FindSubTypes().ToArray();
            return Configs.Save(path, extratypes, new System.IO.StreamWriter(Console.OpenStandardOutput()));
        }
    }

    public partial class TaskRunner
    {
        public bool Load(string path)
        {
            var pm = PersistanceMgr.Instance;
            if (!pm.Load(path))
                return false;

            if (pm.Configs == null || pm.Configs.Cfgs == null)
                return false;

            this.Clear();
            pm.Configs.Cfgs.ForEach(cfg => this.Add(cfg.Create()));

            return true;
        }
    }


}
