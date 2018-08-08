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
    public class GitCloneTask: Task
    {
        [XmlType("GitClone")]
        [Description("GIT clone")]
        public class Config : AConfig
        {
            [XmlAttribute]
            public override string Id { get; set; } = "GIT clone task";

            [XmlElement]
            public override string Command { get; set; } = "git.exe";

            [XmlIgnore]
            public string RepoUrl { get; set; }
            [XmlIgnore]
            public string Branch { get; set; }
            [XmlIgnore]
            public string OutputDir { get; set; }
            
            [XmlElement]
            public override string Args
            {
                get
                {
                    return $"clone \"{RepoUrl}\" --branch \"{Branch}\" \"{OutputDir}\"";
                }

                set
                {
                    //var m = Regex.Match(value, @"clone ([^\s]*) --branch ([^\s]*) (.*)?");
                    var m = Regex.Match(value, @"clone ([\w\W]*) --branch ([^\s]*) (.*)?");
                    if (!m.Success)
                        throw new Exception($"Invalid args string: {value}");

                    RepoUrl = m.Groups[1].Value.Replace("\"", "").Trim();
                    Branch = m.Groups[2].Value.Replace("\"", "").Trim();
                    OutputDir = m.Groups[3].Value.Replace("\"", "").Trim();
                } 
            }

            public override ITask Create()
            {
                return new MSBuildTask().Setup(this);
            }

            public override void Validate()
            {
                //if (!Uri.IsWellFormedUriString(RepoUrl, UriKind.Absolute) && !Directory.Exists(RepoUrl))
                if (!(Uri.IsWellFormedUriString(RepoUrl, UriKind.Absolute) || Directory.Exists(RepoUrl)))
                    throw new InvalidConfigException(this, $"Not valid repository url: {RepoUrl}");

                try
                { Path.GetFullPath(OutputDir); } // is a well defined outputdir?
                catch (Exception)
                { throw new InvalidConfigException(this, $"Invalid output directory: {OutputDir}"); }

                if (!Directory.Exists(Path.GetPathRoot(OutputDir)))
                    throw new InvalidConfigException(this, $"Drive do not exist: {Path.GetPathRoot(OutputDir)}");
            }
        }
    }
}
