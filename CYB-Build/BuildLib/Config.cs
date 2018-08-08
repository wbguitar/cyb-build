using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TaskLib
{
    [Description("Generic")]
    public class Config : AConfig
    {
        [XmlAttribute]
        public override string Id { get; set; }
        [XmlElement]
        public override string Command { get; set; }
        [XmlElement]
        public override string Args { get; set; }

        public override ITask Create()
        {
            return new Task()
                .Setup(this);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Command))
                throw new InvalidConfigException(this, $"Command should not be empty");
        }
    }
}
