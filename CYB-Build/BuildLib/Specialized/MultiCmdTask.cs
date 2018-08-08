using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TaskLib.Specialized
{
    public class MultiTask : Task
    {
        private List<ITask> tasks = new List<ITask>();
        public override ITask Setup(AConfig cfg)
        {
            base.Setup(cfg);
            if (cfg is MultiCmdConfig)
                tasks = (cfg as MultiCmdConfig).Configs
                    .Select(c =>
                    {
                        var task = c.Create();
                        task.OnStdErr += raiseStdErr;
                        task.OnStdOut += raiseStdOut;
                        return task;
                    })
                    .ToList();

            return this;
        }

        public override int Run()
        {
            int ecode = 0;
            foreach (var item in tasks)
            {
                try
                {
                    var p = (item as Task).Process;
                    raiseStdOut(item, p, $"Running task {item.Name}\n{p.StartInfo.FileName} {p.StartInfo.Arguments}");
                    ecode = item.Run();
                    if (ecode != 0)
                        return ecode;
                }
                catch (Exception exc)
                {
                    throw new Exception($"Error running task {item.Name}: {exc.ToString()}");
                }
            }

            return ecode;
        }
    }

    public abstract class MultiCmdConfig : Config
    {
        [XmlIgnore]
        [DisplayName("Elenco task")]
        [Description]
        public virtual List<AConfig> Configs { get; set; } = new List<AConfig>();

        [XmlAttribute]
        public override string Id { get => base.Id; set => base.Id = value; }

        [XmlIgnore]
        [ReadOnly(true)]
        public override string Command
        {
            get
            { return string.Join(Separator, Configs.Select(c => $"{c.Command} {c.Args}")); }
            set
            {
                BuildConfigs(value);
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public override string Args { get => ""; set { } }

        [XmlIgnore]
        [Browsable(false)]
        protected virtual string Separator { get; } = " && ";
        /// <summary>
        /// Implementa la costruzione dei configs in base alla stringa di comando
        /// </summary>
        protected abstract void BuildConfigs(string command);

        protected string[] SplitCommand(string command)
        {
            return command.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
