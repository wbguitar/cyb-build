using TaskLib;
using TaskLib.Specialized;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static TaskLib.Specialized.XCopyTask;
using BL = TaskLib;
using TaskLib.Utils;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Controls;
using System.Linq;

namespace CYB_Build.Model
{


    [XmlType("EnvelopeRemote")]
    public class EnvelopeRemoteCmd : PSExecCfg
    {
        private string envelopeCommand;

        [Category("Remote")]
        [DisplayName("Eseguibile envelope.com")]
        [Description]
        public string EnvelopeCommand
        {
            get { return envelopeCommand; }
            set { envelopeCommand = value; RebuildRmtCmd(); }
        }

        private string envelopePrjFile;
        [Category("Remote")]
        [DisplayName("File di progetto envelope")]
        [Description]
        public string EnvelopePrjFile
        {
            get { return envelopePrjFile; }
            set { envelopePrjFile = value; RebuildRmtCmd(); }
        }

        private void RebuildRmtCmd() => RemoteCommand = $"\"{EnvelopeCommand.Defaults("<envelope.exe>")}\" -p \"{EnvelopePrjFile.Defaults("<prjfile>")}\"";

        [ReadOnly(true)]
        public override string RemoteCommand { get => base.RemoteCommand; set => base.RemoteCommand = value; }
    }


    public class EnvelopeTask : MultiTask
    {
        EnvelopeConfig config;
        public override ITask Setup(AConfig cfg)
        {
            config = cfg as EnvelopeConfig;
            return base.Setup(cfg);
        }
        public override int Run()
        {
            var di = new DirectoryInfo(config.EnvDestFolder.Replace("\\*", ""));
            var files = di.GetFiles();
            var dirs = di.GetDirectories();
            di = new DirectoryInfo(config.EnvSrcFolder.Replace("\\*", ""));
            files = files.Concat(di.GetFiles()).ToArray();
            dirs = dirs.Concat(di.GetDirectories()).ToArray();
            foreach (var f in files)
            {
                f.Delete();
            }
            foreach (var d in dirs)
            {
                d.Delete(true);
            }

            if (!Directory.Exists(config.CriptedFolder.Replace("\\*", "")))
                Directory.CreateDirectory(config.CriptedFolder);

            return base.Run();
        }

        [DisplayName("Remote command executor")]
        [Description]
        [XmlType("Envelope")]
        public class EnvelopeConfig : MultiCmdConfig
        {
            // 1st step: copy bin to envelope source
            //private XCopyConfig preCopy = new XCopyConfig() { Id = "Copy files to crypt" };
            private PowershellCopyConfig preCopy = new PowershellCopyConfig() { Id = "Copy files to crypt" };
            private EnvelopeRemoteCmd envelope = new EnvelopeRemoteCmd() { Id = "Crypting binaries" };
            private PowershellCopyConfig postCopy = new PowershellCopyConfig() { Id = "Copy crypted files" };

            public EnvelopeConfig()
            {
                //base.Configs = new System.Collections.Generic.List<AConfig>()
                //{
                //    (AConfig)preCopy.Clone(),
                //    (AConfig)envelope.Clone(),
                //    (AConfig)postCopy.Clone(),
                //};

                refresh();
            }


            void refresh()
            {
                preCopy = (PowershellCopyConfig)preCopy.Clone();
                envelope = (EnvelopeRemoteCmd)envelope.Clone();
                postCopy = (PowershellCopyConfig)postCopy.Clone();


                var conv = envelope.GetType().GetCustomAttributes(typeof(TypeConverter), true).FirstOrDefault() as TypeConverter;

                base.Configs = new System.Collections.Generic.List<AConfig>()
                {
                    (AConfig)preCopy.Clone(),
                    (AConfig)envelope.Clone(),
                    (AConfig)postCopy.Clone(),
                };
            }

            public override ITask Create()
            {
                return new EnvelopeTask()
                    .Setup(this);
            }

            [XmlAttribute]
            [Category("Local")]
            [DisplayName("Path binari originali")]
            [Description]
            public string BinSources
            {
                get => preCopy.SourceDir;
                set
                {
                    if (!value.EndsWith("\\*"))
                        value += "\\*";

                    preCopy.SourceDir = value;
                    refresh();
                }
            }

            [XmlAttribute]
            [DisplayName("Path input envelope")]
            [Description]
            public string EnvSrcFolder { get => preCopy.OutputDir; set { preCopy.OutputDir = value; refresh(); } }

            [XmlAttribute]
            [DisplayName("Path output envelope")]
            [Description]
            public string EnvDestFolder
            {
                get => postCopy.SourceDir;
                set
                {
                    if (!value.EndsWith("\\*"))
                        value += "\\*";

                    postCopy.SourceDir = value;
                    refresh();
                }
            }

            [XmlAttribute]
            [DisplayName("Path binari criptati")]
            [Description]
            public string CriptedFolder { get => postCopy.OutputDir; set { postCopy.OutputDir = value; refresh(); } }

            //[XmlAttribute]
            //[Description("Eseguibile envelope")]
            //public string EnvelopeCommand { get => envelope.Command?.Replace("\"", ""); set { envelope.Command = $"\"{value.Replace("\"", "")}\""; refresh(); } }
            [XmlElement]
            [DisplayName("Comando envelope")]
            [Description]
            public EnvelopeRemoteCmd EnvelopeCommand { get => envelope; set { envelope = value; refresh(); } }

            //[XmlAttribute]
            //[Description("File parametri envelope")]
            //public string EnvelopeSettings
            //{
            //    get
            //    {
            //        if (string.IsNullOrEmpty(envelope.Args))
            //            return "";

            //        var m = Regex.Match(envelope.Args, @"-p\s*\""([^\""]*)\""");
            //        if (!m.Success)
            //            return string.Empty;

            //        return m.Groups[1].Value;
            //    }

            //    set
            //    {
            //        var val = value.Replace("\"", "");
            //        envelope.Args = $"-p \"{val}\"";
            //        refresh();
            //    }
            //}

            public override void Validate()
            {
                //validatePath(BinSources, "Invalid binary source path");
                validatePath(EnvSrcFolder, "Invalid envelope source path");
                //validatePath(EnvDestFolder, "Invalid envelope destination path");
                validatePath(CriptedFolder, "Invalid cripted destination path");
                //validatePath(EnvelopeCommand, "Invalid envelope command path");
                //validatePath(EnvelopeSettings, "Invalid envelope settings path");

                EnvelopeCommand.Validate();

            }

            protected override string Separator => "\r\n";


            protected override void BuildConfigs(string command)
            {
                //var splits = base.SplitCommand(command);
                //if (splits.Length != 3)
                //    throw new InvalidConfigException(this, $"Wrong formatted command string: {command}");

                //preCopy.Args = splits[1].Replace(preCopy.Command, "").Trim();
                //envelope.Args = splits[2].Replace(envelope.Command, "").Trim();
                //postCopy.Args = splits[3].Replace(postCopy.Command, "").Trim();
            }
        }
    }
}
