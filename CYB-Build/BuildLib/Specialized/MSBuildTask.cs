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
    public class MSBuildTask : Task
    {
        public enum BuildConfig
        {
            Debug,
            Release
        }

        [XmlType("MSBuild")]
        [DisplayName("MS Build")]
        [Description]
        public class Config : AConfig
        {
            [XmlAttribute]
            public override string Id { get; set; } = "MSBuildTask task";
            [XmlElement]
            public override string Command { get; set; }

            [XmlIgnore]
            [DisplayName("Path build result")]
            [Description]
            public string OutputDir { get; set; }
            [XmlIgnore]
            [DisplayName("Path sorgenti C#")]
            [Description]
            public string SolutionPath { get; set; }
            [XmlIgnore]
            [DisplayName("Configurazione build")]
            [Description]
            public BuildConfig Configuration { get; set; } = BuildConfig.Release;

            [XmlElement]
            public override string Args
            {
                get
                {
                    return $"\"{SolutionPath}\\{Defaults.CSSln}\" /p:Configuration={Configuration} /p:OutputPath=\"{OutputDir}\"";
                }

                set
                {
                    var m = Regex.Match(value, @"([^\/]+) \/p\:Configuration\=(.+) \/p\:OutputPath\=(.+)");
                    if (!m.Success)
                        throw new Exception($"Invalid args string: {value}");

                    SolutionPath = m.Groups[1].Value.Replace("\"", "").Replace($"\\{Defaults.CSSln}", "");//Path.GetDirectoryName(m.Groups[1].Value);  //
                    Configuration = (BuildConfig)Enum.Parse(typeof(BuildConfig), m.Groups[2].Value.Replace("\"", ""));
                    OutputDir = m.Groups[3].Value.Replace("\"", "");
                }
            }
            public override ITask Create()
            {
                return new MSBuildTask().Setup(this);
            }

            public override void Validate()
            {
                try
                { Path.GetFullPath(SolutionPath); }
                catch (Exception)
                { throw new InvalidConfigException(this, $"Invalid solution file path: {SolutionPath}"); }

                try
                { Path.GetFullPath(OutputDir); }
                catch (Exception)
                { throw new InvalidConfigException(this, $"Invalid output directory path: {OutputDir}"); }

                //if (Configuration != "Debug" && Configuration != "Release")
                //    throw new InvalidConfigException($"Invalid configuration: {Configuration}, should be Release or Debug");
            }
        }
    }
}
