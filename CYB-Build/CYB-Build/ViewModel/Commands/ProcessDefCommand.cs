using CYB_Build.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using TaskLib.Utils;

namespace CYB_Build.ViewModel.Commands
{
    public class ProcessDefCommand : ACommand
    {
        public ProcessDefCommand()
        {
            App.Current.MainWindow.Closing += delegate
            {
                if (cmdWindow != null)
                    cmdWindow.Close();
            };
        }

        public override bool CanExecute(object parameter)
        {
            return true;// cmdWindow == null || !cmdWindow.IsVisible;
        }

        Window cmdWindow = null;
        public override void Execute(object parameter)
        {
            if (parameter is Button)
            {
                var btn = parameter as Button;
                var action = btn.Content.ToString();
                var title = (string)btn.Tag;
                TaskProcessVM.Instance.SelectedTask = TaskProcessVM.Instance.TaskProcs.FirstOrDefault(t => title == t.Title);
                if (action == "Add")
                {
                    Add(title);
                    btn.Tag = null;
                }
                else if (action == "Edit")
                    Edit(title);
                else if (action == "Remove")
                    Remove(title);

                return;
            }

            // TODO: eliminare non piu` usati
            var parm = (string)parameter;
            if (parm == "Edit")
                Edit();
            else if (parm == "Import")
                Import();
            else if (parm == "Export")
                Export();
        }

        private void Add(string title)
        {

            if (TaskProcessVM.Instance.TaskProcs.Any(tp => tp.Title == title))
            {
                MessageBox.Show($"Task '{title}' already exist", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            TaskProcessVM.Instance.TaskProcs.Add(new TaskProcess() { Title = title });
            TaskProcessVM.Instance.Save();
        }

        private void Export()
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "XML files (*.xml)|*.xml"
            };

            if (!sfd.ShowDialog().Value)
                return;

            var tp = new TaskProcess();
            tp.LoadDefault();
            tp.SaveDefinition(sfd.FileName);

            MainVM.Instance.CurProcessName = tp.Title;
        }

        private void Import()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "XML files (*.xml)|*.xml"
            };
            if (!ofd.ShowDialog().Value)
                return;

            var tp = new TaskProcess();
            tp.LoadDefinition(ofd.FileName);
            tp.SaveDefault();

            MainVM.Instance.CurProcessName = tp.Title;
        }

        private void Edit(string title)
        {
            if (cmdWindow != null && cmdWindow.IsVisible)
                return;

            var tp = TaskProcessVM.Instance.TaskProcs.FirstOrDefault(t => t.Title == title);

            //var tp = new TaskProcess();
            //tp.LoadDefault();

            if (tp != null)
                doEdit(tp);

        }

        private void Remove(string title)
        {
            var res = MessageBox.Show($"Remove task '{title}'?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No)
                return;

            var found = TaskProcessVM.Instance.TaskProcs.FirstOrDefault(t => t.Title == title);
            if (found == null)
                return;

            TaskProcessVM.Instance.TaskProcs.Remove(found);
            TaskProcessVM.Instance.Save();
        }

        private void Edit()
        {
            if (cmdWindow != null && cmdWindow.IsVisible)
                return;

            var tp = new TaskProcess();
            tp.LoadDefault();

            doEdit(tp);
        }

        private void doEdit(TaskProcess tp)
        {
            var idx = TaskProcessVM.Instance.TaskProcs.IndexOf(tp);

            // NB: uso questo invece della versione WPF del toolkit perche` quest'ultima ha un bug del collection editor per cui 
            // viene sparata un'eccezione quando la proprieta` e` una collezione vuota di oggetti (forse nella versione plus a pago e` corretto)
            var pgrid = new System.Windows.Forms.PropertyGrid()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                SelectedObject = tp.Clone()
            };

            var host = new WindowsFormsHost()
            {
                Child = pgrid
            };

            cmdWindow = new Window()
            {
                Width = 600,
                Height = 800,
                //Content = new PropertyGrid() { SelectedObject = bp }
                Content = host,
                //Topmost = true
            };

            cmdWindow.Closed += (s, e) =>
            {
                if (!(pgrid.SelectedObject as TaskProcess).Equals(tp) &&
                MessageBox.Show("Task process was modified, save?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //tp = pgrid.SelectedObject as TaskProcess;
                    //tp.SaveDefault();
                    TaskProcessVM.Instance.TaskProcs[idx] = pgrid.SelectedObject as TaskProcess;
                    TaskProcessVM.Instance.Save();
                }
            };

            RaiseCanExecuteChanged();
            cmdWindow.ShowDialog();

            MainVM.Instance.CurProcessName = tp.Title;

        }

    }
}
