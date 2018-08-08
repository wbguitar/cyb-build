using Catel.Logging;
using CYB_Build.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskLib;
using TaskLib.Utils;

namespace CYB_Build.ViewModel.Commands
{
    public class ImportCommand : ACommand
    {
        BindingManager bm;

        public ImportCommand()
        {
            TaskRunner.Instance.OnStart += () => RaiseCanExecuteChanged();
            TaskRunner.Instance.OnEnd += (ec) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return !TaskRunner.Instance.Running;
        }

        public bool ImportFile(string file)
        {
            //// TaskProcess.LoadDefault needs to be done before importing the config file, because
            //// internally it loads dynamic assemblies that contains AConfig types definitions
            //var bp = new TaskProcess();
            //bp.LoadDefault();

            if (!PersistanceMgr.Instance.Load(file))
            {
                LogVM.Instance.Logger.Error($"Error loading file {file}");
                return false;
            }

            var title = PersistanceMgr.Instance.Configs.TaskProcessTitle;
            if (title.IsNullOrEmpty())
                throw new Exception("Config error: task process without a title!!");

            var bp = TaskProcessVM.Instance.TaskProcs.FirstOrDefault(t => t.Title == title);
            if (bp == null)
            {
                if (MessageBox.Show($"", "", MessageBoxButton.YesNo, MessageBoxImage.Hand) != MessageBoxResult.Yes)
                    return false;

                throw new NotImplementedException(); // TODO
            }

            var configs = PersistanceMgr.Instance.Configs.Cfgs;

            TaskProcessVM.Instance.SelectedTask = bp;

            if (bm != null)
                bm.Dispose();

            bm = new BindingManager(bp);

            var procDef = bp.Setup(configs);
            if (procDef == null)
                return false;

            bm.SetupBindings(procDef);

            // update configs because they could be modified inside bp.Setup
            PersistanceMgr.Instance.Configs.Cfgs = procDef.Keys.ToList();

            ConfigsVM.Instance.Configs =
                new System.Collections.ObjectModel.ObservableCollection<AConfig>(procDef.Keys);

            //ConfigsVM.Instance.CurrentFile = file;

            return true;
        }

        public override void Execute(object parameter)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "XML File (*.xml)|*.xml",
            };
            if (!ofd.ShowDialog().Value)
                return;

            if (!ImportFile(ofd.FileName))
                return;

            Utils.AddRecent(ofd.FileName);
        }
    }
}
