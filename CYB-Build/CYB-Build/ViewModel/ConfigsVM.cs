using TaskLib;
using TaskLib.Specialized;
using Catel.IO;
using CYB_Build.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CYB_Build.ViewModel.Commands;
using System.Windows.Input;

namespace CYB_Build.ViewModel
{

    public class ConfigItem : ANotifyPropertyChanged
    {
        private AConfig config;

        public AConfig Config
        {
            get { return config; }
            set { config = value; RaisePropertyChanged("Config"); }
        }
    }

    public class SetStatusCommand : ACommand
    {
        public SetStatusCommand()
        {
            ConfigsVM.ValueCreated += delegate
            {
                ConfigsVM.Instance.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ConfigsVM.Instance.SelectedConfigs))
                        RaiseCanExecuteChanged();
                };
            };
        }

        public override bool CanExecute(object parameter)
        {
            return ConfigsVM.Instance.SelectedConfigs?.Count > 0;
        }

        public override void Execute(object parameter)
        {

            foreach (var cfg in ConfigsVM.Instance.SelectedConfigs)
            {
                cfg.Status = (TaskLib.TaskStatus)Enum.Parse(typeof(TaskLib.TaskStatus), parameter.ToString());
            }

            ConfigsVM.Instance.RefreshConfigs();
        }
    }

    public class ConfigsVM : ViewModel<ConfigsVM>
    {
        public ICommand Edit => MainVM.Instance.Edit;// { get; set; } = new Edit();
        public ICommand SetStatus { get; set; } = new SetStatusCommand();

        private ObservableCollection<AConfig> configs = new ObservableCollection<AConfig>();
        public ObservableCollection<AConfig> Configs
        {
            get { return configs; }
            set { configs = value; RaisePropertyChanged("Configs"); }
        }

        private string currentFile;

        public string CurrentFile
        {
            get { return currentFile; }
            set { currentFile = value; RaisePropertyChanged("CurrentFile"); }
        }


        private AConfig selected;

        public AConfig SelectedConfig
        {
            get { return selected; }
            set { selected = value; RaisePropertyChanged("SelectedConfig"); }
        }

        private ObservableCollection<AConfig> _selectedConfigs;

        public ObservableCollection<AConfig> SelectedConfigs
        {
            get { return _selectedConfigs; }
            set { _selectedConfigs = value; RaisePropertyChanged("SelectedConfigs"); }
        }


        public void RefreshConfigs()
        {
            Configs = new ObservableCollection<AConfig>(Configs);
        }
    }
}
