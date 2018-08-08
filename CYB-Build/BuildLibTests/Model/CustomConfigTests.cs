using NUnit.Framework;
using CYB_Build.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskLib;

namespace CYB_Build.Model.Tests
{
    [TestFixture()]
    public class CustomConfigTests
    {
        [Test()]
        public void CreateTest()
        {

            var p1 = new CustomTaskParam()
            {
                Name = "1st",
                PType = CustomTaskParam.Type.String,
                Value = "invalid param"
            };

            Assert.Throws(typeof(CustomTaskParam.ParamException), () => p1.Validate(), $"Param should be invalid");
            p1.Value = "validparam";
            Assert.DoesNotThrow(() => p1.Validate(), $"Param should now be valid");

            var p2 = new CustomTaskParam()
            {
                Name = "2nd",
                PType = CustomTaskParam.Type.File,
                Value = @":\test\file.ext",
            };
            Assert.Throws(typeof(CustomTaskParam.ParamException), () => p2.Validate(), $"Param should be invalid");
            p2.Value = @"c:\test\file";
            Assert.Throws(typeof(CustomTaskParam.ParamException), () => p2.Validate(), $"Param should be invalid");
            p2.Value = @"c:\test\file.ext";
            Assert.DoesNotThrow(() => p2.Validate(), $"Param should now be valid");

            var p3 = new CustomTaskParam()
            {
                Name = "3nd",
                PType = CustomTaskParam.Type.Number,
                Value = "aaa",
            };
            Assert.Throws(typeof(CustomTaskParam.ParamException), () => p3.Validate(), $"Param should be invalid");
            p3.Value = "13.3333";
            Assert.DoesNotThrow(() => p3.Validate(), $"Param should now be valid");


            var p4 = new CustomTaskParam()
            {
                Name = "4th",
                PType = CustomTaskParam.Type.Url,
                Value = @":\\www.google.com",
            };

            Assert.Throws(typeof(CustomTaskParam.ParamException), () => p4.Validate(), $"Param should be invalid");
            p4.Value = @"http:\\www.google.com";
            Assert.DoesNotThrow(() => p4.Validate(), $"Param should now be valid");


            var cfg = new CustomConfig()
            {
                Id = "Test",
                Command = "nocmd",
                //Args = "noargs",
                Format = $"",
                Params = new List<CustomTaskParam>()
                {
                    p1, p2,
                    p3, p4
                },
            };


            Assert.Throws<InvalidConfigException>(() => cfg.Validate(), $"Config should be invalid");
            cfg.Format = $@"{{{p1.Name}}} {{{p2.Name}}} {{{p3.Name}}} {{{p4.Name}}}";
            Assert.DoesNotThrow(() => cfg.Validate(), $"Config should be valid");

            Assert.DoesNotThrow(() => cfg.Create(), $"Config should be valid");

            var args = $@"{p1.Value} {p2.Value} {p3.Value} {p4.Value}";
            Assert.AreEqual(cfg.Args, args);

            var p1v = p1.Value;
            var p2v = p2.Value;
            var p3v = p3.Value;
            var p4v = p4.Value;
            cfg.Args = args;
            Assert.AreEqual(p1.Value, p1v);
            Assert.AreEqual(p2.Value, p2v);
            Assert.AreEqual(p3.Value, p3v);
            Assert.AreEqual(p4.Value, p4v);


        }

        [Test()]
        public void ValidateTest()
        {
            throw new NotImplementedException();
        }
    }
}