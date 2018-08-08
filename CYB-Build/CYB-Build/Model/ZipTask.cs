using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TaskLib.Specialized
{
    public class ZipTask : Task
    {
        [Description("Zip compression")]
        public class ZipConfig : AConfig
        {
            [XmlAttribute]
            public override string Id { get; set; } = "ZIP task";

            [XmlIgnore]
            [ReadOnly(true)]
            public override string Command { get; set; } = "zipper.exe";

            [XmlIgnore]
            public override string Args { get => $"\"{SourcePath}\" \"{OutputZip}\""; set { } }

            [XmlAttribute]
            [DisplayName("Input folder or file")]
            [Description]
            public string SourcePath { get; set; }
            [XmlAttribute]
            [DisplayName("Output zip file")]
            [Description]
            public string OutputZip { get; set; }

            public override ITask Create()
            {
                return new ZipTask().Setup(this);
            }

            public override void Validate()
            {
                validatePath(SourcePath, $"Invalid source path: {SourcePath}");
                validatePath(OutputZip, $"Invalid output zip file: {OutputZip}");
            }
        }
    }
}
