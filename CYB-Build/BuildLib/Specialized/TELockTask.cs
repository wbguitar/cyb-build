using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TaskLib.Specialized
{
    public class TELockTask : Task
    {
        [XmlType("TELock")]
        [DisplayName("TE Lock")]
        [Description]
        public class Config : AConfig
        {
            [XmlAttribute]
            public override string Id { get; set; } = "TELock task";

            [XmlElement]
            public override string Command { get; set; }

            //[XmlAttribute]
            //public string CybExe { get; set; } = Defaults.CybExe;

            [XmlIgnore]
            [DisplayName("Path VB build output")]
            [Description]
            public string ExeDir { get; set; } = "";


            [XmlElement]
            public override string Args
            {
                get
                {
                    //return $"-S \"{$"{Path.Combine(ExeDir, Defaults.CybExe)}"}\"";
                    return $"-S {$"{Path.Combine(ExeDir, Defaults.CybExe)}"}";
                }

                set
                {
                    var fullPath = TELockTask.fullPathFromArgs(value).Replace("\"", "");
                    ExeDir = Path.GetDirectoryName(fullPath);
                    //CybExe = Path.GetFileName(fullPath);
                }
            }
            public override ITask Create()
            {
                return new TELockTask().Setup(this);
            }

            public override void Validate()
            {
                // at this point (setup) the file is not already generated
                //var fullPath = Path.Combine(ExeDir, CybExe);
                //if (!File.Exists(fullPath))
                //    throw new InvalidConfigException($"File path is not valid: {fullPath}");
            }
        }

        static string fullPathFromArgs(string args)
        {
            var m = Regex.Match(args, @"-S (.*)");
            if (!m.Success)
                throw new Exception($"Invalid args string: {args}");

            return m.Groups[1].Value;// Replace("\"", "");
        }

        string exePath = "";

        public override ITask Setup(AConfig cfg)
        {
            exePath = fullPathFromArgs(cfg.Args);
            return base.Setup(cfg);
        }

        public override int Run()
        {
            var fsize = new FileInfo(exePath).Length;
            var ecode = base.Run();
            if (ecode != 0) // failed
                return ecode;

            // after run the file size should be reduced
            var newfsize = new FileInfo(exePath).Length;
            if (newfsize >= fsize)
                return -1;

            return ecode;
        }
    }
}
