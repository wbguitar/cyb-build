using CYB_Build.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaskLib;
using TaskLib.Utils;

namespace CYB_Build.ViewModel
{
    [Serializable]
    public class TaskProcessVM : ViewModel<TaskProcessVM>
    {
        Loader<TaskProcessVM> loader = new Loader<TaskProcessVM>();

        public TaskProcessVM()
        {
            if (_inst != null)
            {
                Assemblies = new ObservableCollection<AssemblyItem>(Instance.Assemblies);
                TaskProcs = new ObservableCollection<TaskProcess>(Instance.TaskProcs);
            }
        }
        static TaskProcessVM()
        {
            Loader<TaskProcessVM>.DefaultFile = "TaskProcessDefs.xml";

            var _this = Instance.Load();

            Instance.Assemblies = new ObservableCollection<AssemblyItem>(_this.Assemblies);
            Instance.TaskProcs = new ObservableCollection<TaskProcess>(_this.TaskProcs);
        }

        private ObservableCollection<TaskProcess> taskProcs = new ObservableCollection<TaskProcess>();
        [XmlArray]
        [DisplayName("Elenco definizioni template di processi")]
        public ObservableCollection<TaskProcess> TaskProcs
        {
            get { return taskProcs; }
            set { taskProcs = value; RaisePropertyChanged(nameof(TaskProcs)); }
        }

        public AssemblyItem AddAssembly(Assembly ass)
        {
            var ai = new AssemblyItem(ass);
            Assemblies.Add(ai);
            return ai;
        }

        private TaskProcess selectedTask;
        [XmlIgnore]
        public TaskProcess SelectedTask
        {
            get { return selectedTask; }
            set { selectedTask = value; }
        }

        public TaskProcessVM Load()
        {
            return loader.LoadDefault(this);
        }

        public TaskProcessVM Save()
        {
            return loader.SaveDefault(this);
        }

        ObservableCollection<AssemblyItem> assemblies = new ObservableCollection<AssemblyItem>();
        [XmlArray]
        [DisplayName("Elenco Assembly da caricare")]
        [Description]
        public ObservableCollection<AssemblyItem> Assemblies
        {
            get => assemblies;
            set { assemblies = value; RaisePropertyChanged(nameof(Assemblies)); }
        }

        private Type GetType(string typeName, string assembly)
        {
            var ass = Assembly.Load(new AssemblyName(assembly));
            return ass.GetType(typeName);
        }

        public Type FindType(string typeName)
        {
            foreach (var ai in Assemblies)
            {
                var ass = Assembly.LoadFile(ai.FilePath);
                var found = ass.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(AConfig)))
                    .FirstOrDefault(t => t.FullName == typeName);

                if (found != null)
                    return found;
            }

            return null;
        }

        public override void Execute(object parameter)
        {
            var tp = parameter as TaskProcess;
            if (tp == null)
                return;


            //var parm = (string)parameter;
            //if (parm == "Add")
            //{
            //    throw new NotImplementedException();
            //}
            //else if (parm == "Edit")
            //{
            //    throw new NotImplementedException();
            //}
            //else if (parm == "Remove")
            //{
            //    this.TaskProcs.Remove(SelectedTask);
            //}
        }

        public override bool CanExecute(object parameter)
        {
            var tp = parameter as TaskProcess;
            return tp != null;
        }
    }
}
