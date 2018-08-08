using CYB_Build.Model;
using CYB_Build.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Catel.Data;
using TaskLib;
using TaskLib.Utils;

namespace CYB_Build.View
{
    public enum EditMode
    {
        Add,
        Edit
    }

    /// <summary>
    /// Interaction logic for BindingItemView.xaml
    /// </summary>
    public partial class BindingItemView : Window, INotifyPropertyChanged
    {
        private EditMode mode;

        public EditMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                onPropertyChanged(nameof(Mode));
                onPropertyChanged(nameof(IsAdd));
            }
        }

        public static ValueConverter<EditMode, Visibility> VisibleConverter { get; private set; }

        static BindingItemView()
        {
            VisibleConverter = new ValueConverter<EditMode, Visibility>(
                 (mode) => mode == EditMode.Add ? Visibility.Collapsed : Visibility.Visible);

        }

        public bool IsAdd => Mode == EditMode.Add;

        private TaskProcess taskProcess;

        public TaskProcess TaskProcess
        {
            get { return taskProcess; }
            set
            {
                taskProcess = value;
                var ids = value?.TaskDefs.Select(td => td.Id);
                if (ids != null)
                    TaskIds = new ObservableCollection<string>(ids);
            }
        }


        public BindingItemView()
        {
            InitializeComponent();

            //bp = new TaskProcess().LoadDefault();...
            //var ids = bp.TaskDefs.Select(td => td.Id);


            TaskDefsVM.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TaskDefsVM.Instance.Items))
                {

                }

            };

            TaskDefsVM.Instance.Items.CollectionChanged += delegate
            {
                var tp = TaskProcess.Clone();
                TaskProcessVM.Instance.SelectedTask.TaskDefs = tp.TaskDefs = TaskDefsVM.Instance.Items;

                TaskProcess = tp;
                ResetLocalProps();
                ResetSourceProps();
            };
        }

        public TaskItem TaskItem
        {
            get { return (TaskItem)GetValue(TaskItemProperty); }
            set
            {
                SetValue(TaskItemProperty, value);

                ResetLocalProps();
                onPropertyChanged(nameof(TaskItem));
            }
        }

        // Using a DependencyProperty as the backing store for TaskItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskItemProperty =
            DependencyProperty.Register("TaskItem", typeof(TaskItem), typeof(BindingItemView), new PropertyMetadata(null));




        public BindItem BindItem
        {
            get { return (BindItem)GetValue(BindItemProperty); }
            set { SetValue(BindItemProperty, value); ResetSourceProps(); onPropertyChanged(nameof(BindItem)); }
        }

        // Using a DependencyProperty as the backing store for BindItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindItemProperty =
            DependencyProperty.Register("BindItem", typeof(BindItem), typeof(BindingItemView), new PropertyMetadata(null));




        public ObservableCollection<string> TaskIds
        {
            get { return (ObservableCollection<string>)GetValue(TaskIdsProperty); }
            set { SetValue(TaskIdsProperty, value); onPropertyChanged(nameof(TaskIds)); }
        }
        // Using a DependencyProperty as the backing store for TaskIds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskIdsProperty =
            DependencyProperty.Register("TaskIds", typeof(ObservableCollection<string>), typeof(BindingItemView), new PropertyMetadata(new ObservableCollection<string>()));

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        private ObservableCollection<string> localProps;

        public ObservableCollection<string> LocalProps
        {
            get { return localProps; }
            set { localProps = value; onPropertyChanged(nameof(LocalProps)); }
        }

        private ObservableCollection<string> sourceProps;
        public ObservableCollection<string> SourceProps
        {
            get { return sourceProps; }
            set { sourceProps = value; onPropertyChanged(nameof(SourceProps)); }
        }


        void onPropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void cbSourceId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetSourceProps();
        }

        void ResetLocalProps()
        {
            if (TaskItem == null)
                LocalProps = null;
            else if (TaskItem.ConfigT != null)
                LocalProps = new ObservableCollection<string>(TaskItem.ConfigT.GetProperties().Select(p => p.Name));
        }

        void ResetSourceProps()
        {
            if (BindItem == null)
                return;

            if (BindItem.SourceId.IsNullOrEmpty())
                BindItem.SourceId = TaskIds.First();

            var task = this.TaskProcess.TaskDefs
                .FirstOrDefault(td => td.Id == BindItem.SourceId);
            if (task == null)
            {
                SourceProps = null;
            }
            else
            {
                var type = task.ConfigT;
                SourceProps = new ObservableCollection<string>(type.GetProperties().Select(p => p.Name));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
