using CYB_Build.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskLib;

namespace CYB_Build
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            Activated += delegate
            {
                if (TaskRunner.Instance.Running)
                    return;

                TaskbarItemInfo = null;
            };

            TaskRunner.Instance.OnBeforeRun += (proc) =>
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var count = TaskRunner.Instance.Count;
                    var idx = TaskRunner.Instance.IndexOf(proc) + 1;
                    TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo()
                    {
                        ProgressValue = (idx / (double)count),
                        ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal,
                        Description = $"Running process {idx} of {count} ({proc.Name})"
                    };
                });
            };

            TaskRunner.Instance.OnEnd += (ecode) =>
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    if (ecode == 0)
                    {
                        if (IsActive)
                            Application.Current.Dispatcher.Invoke(delegate { TaskbarItemInfo = null; });
                        else
                            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo()
                            {
                                ProgressValue = 1,
                                ProgressState = System.Windows.Shell.TaskbarItemProgressState.None,
                                Description = $"Process finished with no errors"
                            };
                    }
                });
            };

            TaskRunner.Instance.OnFail += (proc, ecode) =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        if (IsActive)
                            return;

                        var val = (TaskRunner.Instance.IndexOf(proc) + 1) / (double)TaskRunner.Instance.Count;
                        TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo()
                        {
                            ProgressValue = val,
                            ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error,
                            Description = $"Error: {proc.Name} exit with value {ecode}"
                        };
                    });
                };

            Closed += MainWindow_Closed;
            PropertyChanged += MainWindow_PropertyChanged;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // SUX!! for some reason, handling MainVM.Instance.ImportRecent.Executed in the App.OnStartup method hangs the application from exiting
            // here we explicitly call ShutDown
            Application.Current.Shutdown();
        }

        private void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
    }
}
