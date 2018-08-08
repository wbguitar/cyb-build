using CYB_Build.ViewModel;
using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using SIO = System.IO;
using System.Linq;
using TaskLib;

namespace CYB_Build.View
{
    /// <summary>
    /// Interaction logic for ConfigsView.xaml
    /// </summary>
    public partial class ConfigsView : UserControl
    {
        public ConfigsView()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfigsVM.Instance.SelectedConfigs = new System.Collections.ObjectModel.ObservableCollection<AConfig>((sender as ListView).SelectedItems.Cast<AConfig>());
        }
    }

    public class StatusToImageConverter : Singleton<StatusToImageConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //var status = (BuildLib.TaskStatus)value;
                //Bitmap bmp;
                //switch (status)
                //{
                //    case BuildLib.TaskStatus.Todo:
                //        bmp = Properties.Resources.todo;
                //        break;
                //    case BuildLib.TaskStatus.Running:
                //        bmp = Properties.Resources.running;
                //        break;
                //    case BuildLib.TaskStatus.Done:
                //        bmp = Properties.Resources.done;
                //        break;
                //    case BuildLib.TaskStatus.Error:
                //        bmp = Properties.Resources.error;
                //        break;
                //    default:
                //        bmp = Properties.Resources.unknown;
                //        break;
                //}

                //return Convert(bmp);

                var status = (TaskLib.TaskStatus)value;
                string res;
                switch (status)
                {
                    case TaskLib.TaskStatus.Todo:
                        res = @"Resources\todo.png";
                        break;
                    case TaskLib.TaskStatus.Running:
                        res = "Resources/running.gif";
                        break;
                    case TaskLib.TaskStatus.Done:
                        res = "Resources/done.png";
                        break;
                    case TaskLib.TaskStatus.Error:
                        res = "Resources/error.png";
                        break;
                    default:
                        res = "Resources/todo.png";
                        break;
                }

                var curFolder = SIO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                return SIO.Path.Combine(curFolder, res);
            }
            catch (Exception)
            {
                return value;
            }
        }

        BitmapImage Convert(System.Drawing.Bitmap bmp)
        {
            var bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToResourceConverter : Singleton<StatusToResourceConverter>, IValueConverter
    {
        string curFolder;
        public StatusToResourceConverter()
        {
            curFolder = SIO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var status = (TaskLib.TaskStatus)value;
                string res;
                switch (status)
                {
                    case TaskLib.TaskStatus.Todo:
                        res = @"Resources\todo.png";
                        break;
                    case TaskLib.TaskStatus.Running:
                        res = "Resources/running.gif";
                        break;
                    case TaskLib.TaskStatus.Done:
                        res = "Resources/done.png";
                        break;
                    case TaskLib.TaskStatus.Error:
                        res = "Resources/error.png";
                        break;
                    default:
                        res = "Resources/todo.png";
                        break;
                }

                return SIO.Path.Combine(curFolder, res);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
