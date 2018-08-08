using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TaskLib.Specialized
{
    public class XmlDefConfig : AConfig
    {
        public override ITask Create()
        {
            return new XmlDefTask().Setup(this);
        }

        IEnumerable<string> GetParamsNames()
        {
            var ms = Regex.Matches(argsReg, @"\@([^@]*)\@");
            foreach (Match m in ms)
            {
                if (!m.Success)
                    throw new InvalidConfigException(this, $"Badly formatted Args string: {argsReg}");

                yield return m.Groups[1].Value;
            }

        }

        public XmlDefConfig()
        {
            Params.CollectionChanged += Params_CollectionChanged;
        }

        public class Param : IEqualityComparer
        {
            [XmlAttribute]
            public string Name { get; set; }
            [XmlAttribute]
            public string Value { get; set; }

            public new bool Equals(object x, object y)
            {
                return (x as Param).Name == (y as Param).Name;
            }

            public int GetHashCode(object obj)
            {
                return (obj as Param).Name.GetHashCode();
            }
        }

        [XmlArray]
        public ObservableCollection<Param> Params { get; set; } = new ObservableCollection<Param>();

        public override void Validate()
        {
            var parmsNames = GetParamsNames();
        }

        [XmlElement]
        public override string Command { get => base.Command; set => base.Command = value; }

        [XmlIgnore]
        [ReadOnly(true)]
        public override string Args
        {
            get
            {
                var result = Regex.Replace(ArgsReg, @"\@([^@]*)\@", (m) =>
                {
                    var found = this.Params.FirstOrDefault(p => p.Name == m.Groups[1].Value);
                    return found == null ? "" : found.Value;
                });

                return result;
            }
            set
            {
                // nothing to do
            }
        }

        private string argsReg = "";
        [XmlElement]
        public string ArgsReg
        {
            get => argsReg;
            set
            {
                argsReg = value;
                //foreach (var name in GetParamsNames())
                //{
                //    var found = Params.FirstOrDefault(p => p.Name == name);
                //    if (found == null)
                //        Params.Add(new Param() { Name = name });
                //}

                refreshParams();
            }
        }

        bool recurseCall = false;
        void refreshParams()
        {
            if (recurseCall)
                return;

            recurseCall = true;
            var names = GetParamsNames().ToArray();
            // rimuovo i parametri non piu` dichiarati nella stringa
            var toremove = Params.Where(p => !names.Contains(p.Name)).ToList();
            toremove.ForEach(p => Params.Remove(p));

            // aggiungo i nuovi
            foreach (var name in names)
            {
                var found = Params.FirstOrDefault(p => p.Name == name);
                if (found == null)
                {
                    found = new Param() { Name = name, Value = "" };
                    Params.Add(found);
                }
            }

            Params = new ObservableCollection<Param>(Params.Distinct());
            Params.CollectionChanged += Params_CollectionChanged;

            recurseCall = false;
        }

        private void Params_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            refreshParams();
        }

        [XmlElement]
        public override string Id { get => base.Id; set => base.Id = value; }
    }

    public class XmlDefTask : Task
    { }
}
