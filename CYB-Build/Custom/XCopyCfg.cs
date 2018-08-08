using TaskLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Custom
{
    public class XCopyConfig : AConfig
    {
        public override string Args { get => base.Args; set => base.Args = value; }
        public override string Command { get => base.Command; set => base.Command = value; }
        public override string Id { get => base.Id; set => base.Id = value; }

        public string OutputDir { get; set; }
        public string SourceDir { get; set; }

        public override ITask Create()
        {
            return new XCopyTask().Setup(this);
        }

        public override void Validate()
        {
            // TODO
        }
    }

    public class XCopyTask : Task
    {

        
    }    
}
