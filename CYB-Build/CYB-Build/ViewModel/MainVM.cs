using TaskLib;
using Catel.Logging;
using CYB_Build.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid;
using static CYB_Build.Model.TaskProcess;
using CYB_Build.ViewModel.Commands;

namespace CYB_Build.ViewModel
{
    public class MainVM : ViewModel<MainVM>
    {
        public ICommand Run { get; } = new RunCommand();
        public ICommand Edit { get; } = new EditCommand();
        public ICommand NewBuild { get; } = new NewBuildCommand();
        public ICommand Import { get; } = new ImportCommand();
        public ImportRecentCommand ImportRecent { get; }
        public ICommand Export { get; } = new ExportCommand();
        public ICommand Save { get; } = new SaveCommand();
        public ICommand ProcessDef { get; } = new ProcessDefCommand();

        private string curProcessName;

        public string CurProcessName
        {
            get { return curProcessName; }
            set { curProcessName = value; RaisePropertyChanged("CurProcessName"); }
        }

        private ObservableCollection<string> recent;
        public ObservableCollection<string> Recent
        {
            get { return recent; }
            set { recent = value; RaisePropertyChanged("Recent"); }
        }

        public MainVM()
        {
            ImportRecent = new ImportRecentCommand(Import as ImportCommand);

            Recent = new ObservableCollection<string>();
            if (Properties.Settings.Default.Recent != null)
            {
                foreach (var item in Properties.Settings.Default.Recent)
                {
                    Recent.Add(item);
                }
            }

            Recent.CollectionChanged += (s, e) =>
            {
                Properties.Settings.Default.Recent = new StringCollection();
                Properties.Settings.Default.Recent.AddRange(Recent.ToArray());
                Properties.Settings.Default.Save();
            };

            // gestione eccezioni di applicazione
            App.Current.DispatcherUnhandledException += (s, e) =>
            {
                LogVM.Instance.Logger.Error(e.Exception.ToString());
                if (e.Exception is InvalidConfigException)
                {
                    var cfg = (e.Exception as InvalidConfigException).Config;
                    var found = ConfigsVM.Instance.Configs.FirstOrDefault(c => c.Id == cfg.Id);
                    if (found != null)
                    {
                        var idx = ConfigsVM.Instance.Configs.IndexOf(found);
                        var clone = (AConfig)found.Clone();
                        clone.Status = TaskLib.TaskStatus.Error;
                        ConfigsVM.Instance.Configs[idx] = clone;
                    }
                }
                e.Handled = true;
            };

            CurProcessName = new TaskProcess()
                .LoadDefault()
                .Title;

        }
    }

    static class Utils
    {
        static readonly int MAX_COUNT = 5;
        public static void AddRecent(string file)
        {
            var idx = MainVM.Instance.Recent.IndexOf(file);

            if (idx >= 0)
                MainVM.Instance.Recent.RemoveAt(idx);

            MainVM.Instance.Recent.Insert(0, file);
            if (MainVM.Instance.Recent.Count > MAX_COUNT)
                MainVM.Instance.Recent.RemoveAt(MAX_COUNT);
        }

        public static void RemoveRecent(string file)
        {
            var idx = MainVM.Instance.Recent.IndexOf(file);

            if (idx >= 0)
                MainVM.Instance.Recent.RemoveAt(idx);
        }
    }
}
