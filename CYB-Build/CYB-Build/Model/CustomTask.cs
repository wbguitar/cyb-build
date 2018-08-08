using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TaskLib;
using TaskLib.Utils;

namespace CYB_Build.Model
{
    public class CustomTask : Task
    {
    }

    [XmlType]
    public class CustomTaskParam
    {
        public class ParamException : Exception
        {
            public ParamException(string msg, Exception inner = null) : base(msg, inner) { }
        }

        public enum Type
        {
            File,
            Folder,
            Url,
            Number,
            String,
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public Type PType { get; set; }

        [XmlElement]
        public string Value { get; set; }

        public virtual void Validate()
        {
            switch (PType)
            {
                case Type.File:
                    {
                        try
                        {
                            var root = Path.GetPathRoot(Value);
                            if (root.IsNullOrEmpty())
                                throw new ParamException($"Path has empty root: {Value}");

                            var fname = Path.GetFileName(Value);
                            var m = Regex.Match(fname, @"(.*)\.(\w*)");
                            if (fname.IsNullOrEmpty() || !m.Success)
                                throw new ParamException($"Invalid filename: {Value}");
                        }
                        catch (Exception exc)
                        {
                            throw new ParamException($"Invalid path: {Value}, {exc.Message}", exc);
                        }
                    }
                    break;
                case Type.Folder:
                    {
                        try
                        {
                            var root = Path.GetPathRoot(Value);
                            if (root.IsNullOrEmpty())
                                throw new ParamException($"Path has empty root: {Value}");

                            var fname = Path.GetFileName(Value);
                            var m = Regex.Match(fname, @"(.*)\.(\w*)");
                            if (fname.IsNullOrEmpty() || !m.Success)
                                throw new ParamException($"Invalid directory name: {Value}");
                        }
                        catch (Exception exc)
                        {
                            throw new ParamException($"Invalid path: {Value}, {exc.Message}", exc);
                        }
                    }
                    break;
                case Type.Url:
                    {
                        try
                        {
                            var uri = new Uri(Value);
                            var host = Uri.CheckHostName(Value);
                            var scheme = Uri.CheckSchemeName(Value);
                        }
                        catch (Exception e)
                        {
                            throw new ParamException($"Invalid uri format: {Value}, {e.Message}", e);
                        }
                    }
                    break;
                case Type.Number:
                    {
                        try
                        {
                            var val = Double.Parse(Value);
                        }
                        catch (Exception e)
                        {
                            throw new ParamException($"Invalid numeric format: {Value}, {e.Message}", e);
                        }
                    }
                    break;

                case Type.String:
                    {
                        if (Value.Contains(' '))
                            throw new ParamException($"Param should not contain any spaces: '{Value}'");
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void CustomValidate() { }
    }

    public class CustomConfig : AConfig
    {
        public CustomConfig()
        {
            // custom editor
            CYB_Build.ViewModel.ConfigVM.Instance.RegisterControl(this.GetType(), typeof(CYB_Build.View.CustomConfigView));
        }

        [XmlIgnore]
        public override string Args { get => Params2Args(); set => Args2Params(value); }

        [XmlElement]
        public override string Command { get; set; }

        [XmlAttribute]
        public override string Id { get => base.Id; set => base.Id = value; }

        [XmlElement]
        public string Format { get; set; }

        [XmlArray]
        public List<CustomTaskParam> Params { get; set; } = new List<CustomTaskParam>();

        public override ITask Create()
        {
            return new CustomTask().Setup(this);
        }

        public override void Validate()
        {
            if (Params.Select(p => p.Name).Distinct().Count() < Params.Count)
                throw new InvalidConfigException(this, $"Params names should be unique");

            foreach (var p in Params)
            {
                try
                {
                    p.Validate();
                }
                catch (Exception e)
                {
                    throw new InvalidConfigException(this, $"Invalid config:\n{e.Message}", e);
                }
            }

            var args = Args; // valida la conversione
        }

        protected string Params2Args()
        {
            var s = Format;
            int count = 0;
            foreach (var p in Params)
            {
                var tok = $"{{{p.Name}}}";
                if (!s.Contains(tok))
                    throw new InvalidConfigException(this, $"Param {p.Name} not found in format string ({Format})");

                s = s.Replace(tok, p.Value);
                count++;
            }

            if (count != Params.Count)
                throw new InvalidConfigException(this, $"Invalid parameters count ({count}), for format string '{Format}'");

            return s;
        }

        protected void Args2Params(string args)
        {
            var regex = Format;
            var parms = new SortedDictionary<int, CustomTaskParam>(); // ordinati in base all'indice
            foreach (var p in Params)
            {
                var tok = $"{{{p.Name}}}";
                var idx = regex.IndexOf(tok);
                if (idx < 0)
                    throw new InvalidConfigException(this, $"Param {p.Name} not found in format string ({Format})");

                regex = regex.Replace(tok, @"(.*)");

                parms[idx] = p;
            }

            if (parms.Count != Params.Count)
                throw new InvalidConfigException(this, $"Invalid parameters count ({parms.Count}), for format string '{Format}'");

            var m = Regex.Match(args, regex);
            if (!m.Success ||
                m.Groups.Count != Params.Count + 1)
                throw new InvalidConfigException(this, $"Invalid args string:\nargs: {args}format: '{Format}'");

            var parmValues = parms.Values.ToArray();
            for (int i = 0; i < Params.Count; i++)
            {
                var pValue = m.Groups[i + 1].Value;
                var p = this.Params.FirstOrDefault(parm => parm.Name == parmValues[i].Name);
                p.Value = pValue;
            }
        }
    }
}
