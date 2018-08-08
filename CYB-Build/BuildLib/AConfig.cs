using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TaskLib
{
    public class InvalidConfigException : Exception
    {
        public AConfig Config { get; private set; }
        public InvalidConfigException(AConfig cfg, string msg, Exception inner = null) : base(msg, inner) { Config = cfg; }
    }

    public enum TaskStatus
    {
        Todo,
        Running,
        Done,
        Error
    }

    public abstract class AConfig : ICloneable
    {
        [Category("Basic")]
        [DisplayName("Stato del task")]
        [Description]
        //[ReadOnly(true)]
        [XmlAttribute]
        public TaskStatus Status { get; set; }

        [Category("Basic")]
        [DisplayName("Identificativo univoco di un task (descrizione)")]
        [Description]
        [ReadOnly(true)]
        [XmlIgnore] // attributes should be added to derived classes
        public virtual string Id { get; set; }

        [Category("Basic")]
        [DisplayName("Comando da eseguire (percorso completo se il comando non e` nel PATH)")]
        [Description]
        [XmlIgnore]
        public virtual string Command { get; set; }

        [Category("Basic")]
        [DisplayName("Stringa rappresentante la riga di comando")]
        [Description]
        [XmlIgnore]
        public virtual string Args { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public abstract ITask Create();

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidConfigException">Thrown if the validation gone wrong</exception>
        public abstract void Validate();

        protected void validatePath(string path, string errMsg)
        {
            try
            { Path.GetFullPath(path); }
            catch (Exception)
            { throw new InvalidConfigException(this, $"{errMsg}: {path}"); }
        }

    }
}
