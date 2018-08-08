using TaskLib;
using TaskLib.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static TaskLib.Specialized.XCopyTask;

namespace Custom
{
    public class GlobalTaskConfig : MultiCmdConfig
    {
        public GlobalTaskConfig()
        {
            //CYB_Build.ViewModel.ConfigVM.Instance.RegisterControl(this.GetType(), typeof(CYB_Build.View.FinalTaskConfigView));
        }

        void createConfigs()
        {

        }

        [Category("C# Git")]
        [DisplayName("Repo URL")]
        [Description]
        public string CSharpRepo { get; set; }
        [Category("C# Git")]
        [DisplayName("Branch")]
        [Description]
        public string CSharpBranch { get; set; }
        [Category("C# Git")]
        [DisplayName("Libs repo URL")]
        [Description]
        public string CSharpLibsRepo { get; set; }
        [Category("C# Git")]
        [DisplayName("Libs branch")]
        [Description]
        public string CSharpLibsBranch { get; set; }


        //[Category("MS Build")]
        //[Description("Solution folder")]
        //public string VSSolutionDir { get; set; }

        [Category("VB Git")]
        [DisplayName("Repo URL")]
        [Description]
        public string VBRepo { get; set; }
        [Category("VB Git")]
        [DisplayName("Branch")]
        [Description]
        public string VBBranch { get; set; }

        private int issue;
        [Category("Setup")]
        [DisplayName("Numero fascicolo")]
        [Description]
        public int Issue
        {
            get { return issue; }
            set { issue = value; createConfigs(); }
        }



        private string commessa;
        [Category("Setup")]
        [DisplayName("Id commessa")]
        [Description]
        public string Commessa
        {
            get { return commessa; }
            set { commessa = value; createConfigs(); }
        }

        [Category("Setup")]
        [XmlIgnore]
        //[Browsable(false)]
        [ReadOnly(true)]
        public int Year
        {
            get
            {
                var m = RegexCommessa();
                if (!m.Success)
                    return 0;

                return DateTime.ParseExact(m.Groups[2].Value, "yy", null).Year;
            }
        }

        Match RegexCommessa()
        {
            return Regex.Match(Commessa, @"(M-P|M-R|M-I)(\d\d)(\d\d\d)");
        }

        [Category("Setup")]
        [XmlIgnore]
        //[Browsable(false)]
        [ReadOnly(true)]
        public string CatCommessa
        {
            get
            {
                var m = RegexCommessa();
                if (!m.Success)
                    return "";

                return m.Groups[1].Value;
            }
        }

        [Category("Setup")]
        [XmlIgnore]
        //[Browsable(false)]
        [ReadOnly(true)]
        public int NumCommessa
        {
            get
            {
                var m = RegexCommessa();
                if (!m.Success)
                    return 0;

                return int.Parse(m.Groups[3].Value);
            }
        }

        public override ITask Create()
        {
            return new GlobalTask().Setup(this);
        }

        protected override string Separator => "\r\n";

        public override void Validate()
        {
            //throw new NotImplementedException();
            // TODO;
        }

        protected override void BuildConfigs(string command)
        {
            // NOTHING TO DO;
        }
    }

    public class GlobalTask : MultiTask
    { }
}
