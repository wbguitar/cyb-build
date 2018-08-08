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
    public class VBTask : Task
    {
        [XmlType("VBBuild")]
        [DisplayName("Visual Basic 6")]
        [Description]
        public class Config : AConfig
        {
            public Config()
            { }

            [XmlAttribute]
            public override string Id { get; set; } = "VB6 task";

            [XmlElement]
            public override string Command { get; set; }

            //[XmlAttribute]
            //public string CybExe { get; set; } = Defaults.CybExe;

            [XmlIgnore]
            [DisplayName("Path sorgenti VB")]
            [Description]
            public string ProjectPath { get; set; }
            [XmlIgnore]
            [DisplayName("Path build result")]
            [Description]
            public string OutDir { get; set; }
            [XmlIgnore]
            [DisplayName("File build log")]
            [Description]
            public string StdoutFile { get; set; }

            [XmlElement]
            public override string Args
            {
                get
                {
                    return $"/make \"{ProjectPath}\\{Defaults.VBProj}\" /outdir \"{OutDir}\"" + (string.IsNullOrEmpty(StdoutFile) ? "" : $" /out \"{StdoutFile}\"");
                }

                set
                {
                    try
                    {
                        var split = value.Split(new string[] { @"/make", @"/outdir", @"/out" }, StringSplitOptions.RemoveEmptyEntries);
                        ProjectPath = split[0].Replace("\"", "").Replace($"\\{Defaults.VBProj}", "").Trim();
                        OutDir = split[1].Replace("\"", "").Trim();

                        if (split.Length > 2)
                            StdoutFile = split[2].Replace("\"", "").Trim();
                    }
                    catch (Exception)
                    {
                        throw new InvalidConfigException(this, $@"Invalid Args string: {value}");
                    }

                    ////var m = Regex.Match(value, @"\/make ([\s\S]*) \/outdir ([\s\S]*)(\s*[\s\S]*)?");
                    //var m = Regex.Match(value, @"\/make (.*\/) \/outdir (.*\/)(\s*[\s\S]*)?");
                    //if (!m.Success)
                    //    throw new Exception($"Invalid args string: {value}");

                    //ProjectPath = m.Groups[1].Value;
                    //OutDir = m.Groups[2].Value;
                }
            }

            public override ITask Create()
            {
                return new VBTask().Setup(this);
            }

            public override void Validate()
            {
                var exePath = Path.Combine(OutDir, Defaults.CybExe);

                try
                { Path.GetFullPath(OutDir); }
                catch (Exception e)
                { throw new InvalidConfigException(this, $"Invalid VB output directory path: {OutDir}, {e.Message}"); }

                try
                { Path.GetFullPath(ProjectPath); }
                catch (Exception e)
                { throw new InvalidConfigException(this, $"Invalid VB project path: {ProjectPath}, {e.Message}"); }


                if (Directory.Exists(OutDir))
                {
                    // removes all cyb500n.* files
                    var cybFiles = Directory.GetFiles(OutDir, Defaults.CybExe.Replace("exe", "*"));
                    foreach (var f in cybFiles)
                    {
                        if (File.Exists(f))
                            File.Delete(f);
                    }
                }

                //if (!Directory.Exists(OutDir))
                //    throw new InvalidConfigException($"Invalid output directory: {OutDir}");
            }
        }

        Config config;
        FileSystemWatcher watcher;
        public override ITask Setup(AConfig cfg)
        {
            config = cfg as Config;
            cfg.Validate();

            var tempPath = Path.GetTempPath();
            var outPath = Path.GetRandomFileName();
            //var outPath = Path.GetTempFileName();
            var fullPath = Path.Combine(tempPath, outPath);

            cfg.Args = $"{cfg.Args} /out {fullPath}";
            //cfg.Args = $"{cfg.Args} /out {outPath}";

            if (watcher != null)
                watcher.Dispose();

            watcher = new FileSystemWatcher(tempPath, $"*{Path.GetExtension(outPath)}")
            //watcher = new FileSystemWatcher(Path.GetDirectoryName(outPath), $"*{Path.GetExtension(outPath)}")//Path.GetFileName(outPath))
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName |
                NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security,
            };

            var fileContent = "";
            var watcherHandler = new Action<FileSystemEventArgs>((e) =>
            {
                if (e.ChangeType == WatcherChangeTypes.Deleted || !Path.Equals(e.FullPath, fullPath))
                    return;

                using (var sr = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var content = new StreamReader(sr).ReadToEnd();
                    if (string.IsNullOrEmpty(content))
                        return;

                    var newContent = "";
                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        newContent = content.Replace(fileContent, "");
                    }
                    else
                        newContent = content;

                    RaiseOnOut(proc, newContent);
                    fileContent = content;
                }
            });

            //watcher.Renamed += (s, e) => watcherHandler(e);
            //watcher.Created += (s, e) => watcherHandler(e);
            watcher.Changed += (s, e) => watcherHandler(e);

            return base.Setup(cfg);
        }

        public override int Run()
        {
            if (!Directory.Exists(config.OutDir))
                Directory.CreateDirectory(config.OutDir);

            return base.Run();
        }
    }
}
