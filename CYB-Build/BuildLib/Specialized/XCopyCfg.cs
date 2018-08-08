using TaskLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Diagnostics;
using TaskLib.Utils;

namespace TaskLib.Specialized
{
    public class PowershellCopyConfig : AConfig
    {
        [XmlAttribute]
        public override string Command { get; set; } = "powershell";

        [XmlAttribute]
        public override string Id { get; set; }

        public override string Args { get => BuildArgs(); set { } }

        private string BuildArgs()
        {
            var source = SourceDir.Defaults("<sourcedir>");
            //if (!source.EndsWith("\\*"))
            //    source += "\\*";

            return $"-command copy \'{source}\' \'{OutputDir.Defaults("<sourcedir>")}\' -recurse -verbose -force";
        }

        [XmlAttribute]
        [DisplayName("Path destinazione")]
        [Description]
        public string OutputDir { get; set; }
        [XmlAttribute]
        [DisplayName("Path di origine")]
        [Description]
        public string SourceDir { get; set; }

        public override ITask Create()
        {
            return new Task().Setup(this);
        }

        public override void Validate()
        {
            //validatePath(SourceDir, $"Invalid source folder path: {SourceDir}");
            validatePath(OutputDir, $"Invalid destination folder path: {OutputDir}");
        }
    }

    public class XCopyTask : Task
    {
        [Description("xcopy task")]
        public class XCopyConfig : AConfig
        {
            [XmlAttribute]
            public override string Command { get; set; } = "xcopy";

            [XmlAttribute]
            public override string Id { get; set; }

            [XmlAttribute]
            [DisplayName("Path destinazione")]
            [Description]
            public string OutputDir { get; set; }
            [XmlAttribute]
            [DisplayName("Path di origine")]
            [Description]
            public string SourceDir { get; set; }

            public static string Options = @"/b /v /y";
            //public static string Options = @"/c /d /e /h /i /k /q /r /s /x /y";

            [XmlIgnore]
            public override string Args
            {
                get
                {
                    return $@"""{SourceDir}"" ""{OutputDir}"" {Options}";
                }
                set
                {
                    var m = Regex.Match(value, $@"\""([^\""]*)\""\s*\""([^\""]*)\""\s*{Options}");
                    if (!m.Success)
                        throw new Exception($"Invalid args string: {value}");

                    SourceDir = m.Groups[1].Value;
                    OutputDir = m.Groups[2].Value;
                }
            }

            public override ITask Create()
            {
                return new XCopyTask().Setup(this);
            }

            public override void Validate()
            {
                try
                { Path.GetFullPath(OutputDir); }
                catch (Exception)
                { throw new InvalidConfigException(this, $"Invalid destination folder path: {OutputDir}"); }

                try
                { Path.GetFullPath(SourceDir); }
                catch (Exception)
                { throw new InvalidConfigException(this, $"Invalid source folder path: {SourceDir}"); }
            }
        }
    }
}
