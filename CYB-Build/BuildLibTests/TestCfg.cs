using TaskLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLib.Tests
{
    public class PingCfg : AConfig
    {
        public override string Command { get; set; } = "ping.exe";

        public override string Args { get; set; } = @"google.it";

        public override string Id => "Ping";

        public override ITask Create()
        {
            throw new NotImplementedException();
        }

        public override void Validate()
        {
            if (!Uri.IsWellFormedUriString(Args, UriKind.RelativeOrAbsolute))
                throw new InvalidConfigException(this, $"Not valid uri: {Args}");
        }
    }

    public class IpconfigCfg : AConfig
    {

        public override string Command => "ipconfig.exe";

        public override string Args { get; set; } = "";

        public override string Id => "IPconfig";

        public override ITask Create()
        {
            throw new NotImplementedException();
        }

        public override void Validate()
        {
            
        }
    }

    public class MSBuildCfg : AConfig
    {
        public override string Command => "msbuild.exe";

        public override string Args { get; set; } = @"E:\dev\marini\m-p17022\UTS170022F001_SourceC#.9.6.23.4\CybertronicV9.6\Cybertronic\CybertronicSln.sln";

        public override string Id => "MSbuild";

        public override ITask Create()
        {
            throw new NotImplementedException();
        }

        public override void Validate()
        {
            //if (!File.Exists(ExePath))
            //    throw new InvalidConfigException($"Unable to find MSbuild exe: {ExePath}");
        }
    }

    public class VBBuildCfg : AConfig
    {
        public override string Command => @"""C:\Program Files (x86)\Microsoft Visual Studio\VB98\VB6.EXE""";

        public const string StdOutFile = @"e:\vbout.txt";

        public override string Args { get; set; } = $@"/make ""E:\dev\marini\M-R17213\UTS170213F001_SourcePC.9.6.44.1\Source\ComCP240.vbp"" /outdir e:\ /out {StdOutFile}";

        public override string Id => "VB6Build";

        public override void Validate()
        {
            //if (!File.Exists(ExePath))
            //    throw new InvalidConfigException($"Unable to find VB6 exe: {ExePath}");
        }

        public override ITask Create()
        {
            throw new NotImplementedException();
        }
    }

    public class TElockCfg : AConfig
    {
        public override string Command => @"C:\Users\btifac\Desktop\telock98\telock.exe";

        public override string Args { get; set; } = @"-S e:\Cyb500N.exe";

        public override string Id => "TELock";

        public override ITask Create()
        {
            throw new NotImplementedException();
        }

        public override void Validate()
        {
            if (!File.Exists(Command))
                throw new InvalidConfigException(this, $"Unable to find TElock exe: {Command}");
        }
    }
}
