using TaskLib;
using TaskLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using CYB_Build.View;
using System.Windows;
using CYB_Build.ViewModel;
using System.Windows.Input;

namespace CYB_Build.Model
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BindItem
    {
        [XmlAttribute]
        [DisplayName("Proprietà locale")]
        [Description]
        public string LocalProp { get; set; }
        [XmlAttribute]
        [DisplayName("Proprietà remota")]
        [Description]
        public string SourceProp { get; set; }
        [XmlAttribute]
        [DisplayName("Id task remoto")]
        [Description]
        public string SourceId { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as BindItem;
            return other != null && other.LocalProp == LocalProp &&
                   other.SourceId == SourceId &&
                   other.SourceProp == SourceProp;
        }
    }

    class CustomEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                //var items = (ObservableCollection<TaskItem>)value;
                //var f = new Form();
                //var editor = new TaskDefsEditor((ObservableCollection<TaskItem>)value)
                //{ Dock = DockStyle.Fill, };
                //f.Controls.Add(editor);
                ////svc.ShowDialog(f);
                //f.FormClosing += delegate
                //{
                //    value = new ObservableCollection<TaskItem>(editor.Items);
                //};
                //f.Show();

                TaskDefsVM.Instance.Items = (ObservableCollection<TaskItem>)value;
                var w = new Window()
                {
                    Width = 600,
                    Content = new TaskDefsView(),
                };

                w.Closing += delegate
                {
                    if (TaskDefsVM.Instance.Items.SequenceEqual((ObservableCollection<TaskItem>)value))
                        return;

                    if (System.Windows.MessageBox.Show("Object modified, save?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        value = new ObservableCollection<TaskItem>(TaskDefsVM.Instance.Items);
                };
                w.Show();

            }
            return value;
        }
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Bindings : ObservableCollection<BindItem>
    {
        public Bindings() : base() { }
        public Bindings(IEnumerable<BindItem> collection) : base(collection) { }
    }

    [Serializable]
    public class TaskItem : ANotifyPropertyChanged
    {
        public TaskItem() { }
        public TaskItem(AConfig cfg)
        {
            Config = cfg.GetType().FullName;
            Id = cfg.Id;
        }

        public BindItem AddBinding(TaskItem source, string localProp, string srcProp)
        {
            if (source == this && localProp == srcProp)
                throw new InvalidOperationException("Cannot bind to self!");

            var localType = ConfigT;
            if (localType == null)
                throw new InvalidOperationException($"Type {Config} not found on loaded assemblies");

            var sourceType = source.ConfigT;
            if (sourceType == null)
                throw new InvalidOperationException($"Type {Config} not found on loaded assemblies");

            if (!localType.GetProperties()
                .Any(prop => prop.Name == localProp))
                throw new InvalidOperationException($"Local property {localProp} not found");

            if (!sourceType.GetProperties()
                .Any(prop => prop.Name == srcProp))
                throw new InvalidOperationException($"Source property {srcProp} not found");

            var bi = new BindItem()
            {
                SourceId = source.Id,
                SourceProp = srcProp,
                LocalProp = localProp
            };

            Bindings.Add(bi);
            return bi;
        }

        /// <summary>
        /// Returns the Type object found through the Config property
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public Type ConfigT
        {
            get
            {
                return Config.FindType(typeof(AConfig));
            }
            set
            {
                Config = value.FullName;
            }
        }

        [XmlAttribute]
        [DisplayName("Identificativo processo")]
        [Description]
        //[ReadOnly(true)]
        public string Id { get; set; }

        private string config;
        /// <summary>
        /// Full name of the type of the config associated to this build item
        /// </summary>
        [XmlAttribute]
        //[Browsable(false)]
        public string Config
        {
            get { return config; }
            set { config = value; RaisePropertyChanged(nameof(Config)); }
        }

        [XmlArray]
        [DisplayName("Elenco bindings")]
        [Description]
        [Editor(typeof(CollectionEditor), typeof(ITypeEditor))]

        public Bindings Bindings { get; set; } = new Bindings();

        public override bool Equals(object obj)
        {
            var other = obj as TaskItem;
            return other != null &&
                other.Id == Id &&
                other.Bindings.SequenceEqual(Bindings) &&
                other.Config == Config;
        }

        public TaskItem Clone()
        {
            return new TaskItem()
            {
                Bindings = new Bindings(this.Bindings),
                Config = this.Config,
                Id = this.Id
            };
        }
    }

    [Serializable]
    public class AssemblyItem
    {
        public AssemblyItem() { }
        public AssemblyItem(Assembly ass) { FilePath = Catel.IO.Path.GetRelativePath(ass.Location); }
        private Assembly assembly = null;

        [XmlIgnore]
        public Assembly Assembly { get => assembly; }

        private string fpath;
        [XmlAttribute]
        public string FilePath
        {
            get => fpath;
            set
            {
                if (System.IO.File.Exists(value))
                {
                    assembly = Assembly.LoadFile(System.IO.Path.GetFullPath(value));
                    fpath = value;
                }
                else
                {
                    assembly = null;
                    fpath = "";
                }
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as AssemblyItem;
            return other != null && other.Assembly == this.Assembly &&
                   other.FilePath == this.FilePath;
        }
    }

    [Serializable]
    public class TaskProcess
    {
        private static readonly string file = "TaskProcessDef.xml";

        [XmlAttribute]
        public string Title { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as TaskProcess;
            return other != null &&
                //this.Assemblies.SequenceEqual(other.Assemblies) &&
                this.TaskDefs.SequenceEqual(other.TaskDefs) &&
                other.Title == this.Title;
        }

        public TaskProcess LoadDefault()
        {
            return LoadDefinition(file);
        }

        public TaskProcess SaveDefault()
        {
            return SaveDefinition(file);
        }

        public TaskProcess LoadDefinition(string fpath)
        {
            if (!File.Exists(fpath))
                throw new FileNotFoundException($"Cannot find build process definitions file ({fpath})");

            var bp = this.Load(fpath);
            this.TaskDefs = bp.TaskDefs;
            //this.Assemblies = bp.Assemblies;
            this.Title = bp.Title;
            return this;
        }

        public TaskProcess SaveDefinition(string fpath)
        {
            this.Save(fpath);
            return this;
        }

        public void Clear()
        {
            //this.Assemblies.Clear();
            this.TaskDefs.Clear();
        }

        public TaskItem AddBuildDef(AConfig cfg)
        {
            var bi = new TaskItem(cfg);
            TaskDefs.Add(bi);
            return bi;
        }

        [XmlArray]
        [Editor(typeof(CustomEditor), typeof(UITypeEditor))]
        public ObservableCollection<TaskItem> TaskDefs { get; set; } = new ObservableCollection<TaskItem>();

        private Type GetType(string typeName, string assembly)
        {
            var ass = Assembly.Load(new AssemblyName(assembly));
            return ass.GetType(typeName);
        }


        public TaskItem this[string id] => TaskDefs.FirstOrDefault(bd => bd.Id == id);

        public class DefinitionTable : Dictionary<AConfig, TaskItem> { }

        /// <summary>
        /// From the configs in the <see cref="TaskDefs"/> creates related tasks and return the config-task table
        /// </summary>
        /// <returns>Generated config-task table</returns>
        public DefinitionTable Generate()
        {
            var dt = new DefinitionTable();
            foreach (var def in TaskDefs)
            {
                var type = def.ConfigT;
                if (type == null)
                    throw new Exception($"Wrong config type should derive from AConfig: {def.Config}");

                var cfg = type.GetConstructor(Type.EmptyTypes).Invoke(null) as AConfig;
                cfg.Id = def.Id;
                dt[cfg] = def;
            }

            return dt;
        }

        class EqComp : IEqualityComparer<AConfig>
        {
            public bool Equals(AConfig x, AConfig y)
            {
                return x.GetType() == y.GetType();
            }

            public int GetHashCode(AConfig obj)
            {
                return obj.GetHashCode();
            }
        }

        public DefinitionTable Setup(IEnumerable<AConfig> configs)
        {
            // generates config-task table
            var dict = this.Generate();

            // is configs different from the task defs template?
            if (!dict.Keys.SequenceEqual(configs, new EqComp()))
            {
                //throw new InvalidOperationException("Config collection must be compatible with the build process definition");
                if (System.Windows.MessageBox.Show("Current config configuration is not compatible with build process definition, update?", "",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                    return null;

                var cfgs = dict.Keys.ToArray();
                for (int i = 0; i < cfgs.Length; i++)
                {
                    var found = configs.FirstOrDefault(cfg => cfg.Id == cfgs[i].Id);
                    if (found != null)
                        cfgs[i] = found.Clone() as AConfig;
                }

                configs = cfgs;
            }

            var keys = dict.Keys.ToArray();
            for (int i = 0; i < configs.Count(); i++)
            {
                var cfg = configs.ElementAt(i);
                var key = keys[i];
                var val = dict[key];
                dict.Remove(key);
                dict.Add(cfg, val);
            }

            return dict;
        }
    }

}
