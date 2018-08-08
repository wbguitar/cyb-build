using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using CYB_Build.View;
using QC = QuickConverter;
using CYB_Build.ViewModel;
using CYB_Build.ViewModel.Commands;

namespace CYB_Build
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            QC.EquationTokenizer.AddNamespace(typeof(object));
            QC.EquationTokenizer.AddNamespace(typeof(Visibility));
            QC.EquationTokenizer.AddNamespace(typeof(EditMode));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // various configs, here the singletons have been all correctly initialized
            var setFileName = new Action(delegate
            {
                // update current config file, that is stored tab item header
                var header = TabCtrlVM.Instance.SelectedItem?.Header;
                if (header?.ToString() != TabCtrlVM.DefaultHeader &&
                    System.IO.File.Exists(header?.ToString()))
                    ConfigsVM.Instance.CurrentFile = header.ToString();
                else
                    ConfigsVM.Instance.CurrentFile = null;
            });

            // when changing tabitem
            TabCtrlVM.Instance.PropertyChanged += (s, ea) =>
            {
                if (ea.PropertyName == nameof(TabCtrlVM.Instance.SelectedItem))
                {
                    setFileName();
                }
            };

            // when loading file
            MainVM.Instance.ImportRecent.Executed += setFileName;

            // args processing
            if (e.Args.Length > 0 )
            {
                if (e.Args.Contains("-c") && AttachConsole(ATTACH_PARENT_PROCESS))
                {
                    Console.WriteLine("Console running");
                    FreeConsole();
                    Shutdown();
                }
                else if (e.Args[0] == "this" && e.Args.Length > 1)
                {
                    var args = e.Args.ToList().GetRange(1, e.Args.Length - 1);

                }
            }
        }

        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

    }

}
