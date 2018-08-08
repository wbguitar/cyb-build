using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskLib;
using System.ComponentModel;
using System.Xml.Serialization;
using TaskLib.Utils;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace CYB_Build.Model
{

    [DisplayName("Remote command executor")]
    [Description]
    //[Editor(typeof(Editor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlType("PSExec")]
    public class PSExecCfg : AConfig
    {
        public class Editor : UITypeEditor
        {
            public Editor()
            {

            }
            //public class Control : UserControl
            //{
            //    public Control(object val)
            //    {
            //        Content = new Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid()
            //        {
            //            SelectedObject = val
            //        };
            //    }
            //}

            public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                var editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                var pgrid = new System.Windows.Forms.PropertyGrid()
                {
                    SelectedObject = value
                };
                editorService.DropDownControl(pgrid);

                value = pgrid.SelectedObject;

                //if (provider != null)
                //{
                //    editorService =
                //        provider.GetService(
                //        typeof(IWindowsFormsEditorService))
                //        as IWindowsFormsEditorService;
                //}

                //if (editorService != null)
                //{
                //    var selectionControl =
                //        new LightShapeSelectionControl(
                //        (MarqueeLightShape)value,
                //        editorService);

                //    editorService.DropDownControl(selectionControl);

                //    value = selectionControl.LightShape;
                //}

                return value;
            }
        }

        [Category("Auth")]
        [DisplayName("Username")]
        [Description]
        public string User { get; set; }
        [Category("Auth")]
        [DisplayName("Password")]
        [Description]
        public string Password { get; set; }
        [Category("Remote")]
        [DisplayName("Directory corrente (nella macchina remota)")]
        [Description]
        public string CurrentFolder { get; set; }
        [Category("Remote")]
        [DisplayName("Macchina remota")]
        [Description]
        public string RemoteMachine { get; set; }
        [Category("Remote")]
        [DisplayName("Comando remoto (nella macchina remota)")]
        [Description]
        public virtual string RemoteCommand { get; set; }

        [ReadOnly(true)]
        public override string Command { get; set; } = "psexec";

        [ReadOnly(true)]
        public override string Args { get => BuildArgs(); set { } }

        private string BuildArgs()
        {
            /*
             * Formato:
             * psexec <machine> -h -u <username> -p <pwd> -w <currentfld> <remotecmd>
             * NB: i path devono essere relativi alla macchina remota!!
             * es: psexec \\uts31 -h -u "marini\uts" -p uts -w "C:\Users\uts\Desktop" "C:\Program Files\Gemalto Sentinel\Sentinel LDK\VendorTools\VendorSuite\envelope.com" -p "Cybertronic500 v9.6.25_Envelope_v7.5.prjx"
             */
            var sb = new StringBuilder();
            // machine
            sb.Append($"\\\\{RemoteMachine.Defaults("<remotemachine>")} -h ");
            // authentication
            sb.Append($"-u {User.Defaults("<username>")} -p {Password.Defaults("<password>")} ");
            // currentfolder (if added)
            if (!CurrentFolder.IsNullOrEmpty())
                sb.Append($"-w \"{CurrentFolder}\" ");
            // remote command
            sb.Append($"{RemoteCommand.Defaults("<remotecommand>")}");

            return sb.ToString();
        }

        public override ITask Create()
        {
            return new PSexecTask().Setup(this);
        }

        public override void Validate()
        {

        }
    }

    public class PSexecTask : Task { }
}
