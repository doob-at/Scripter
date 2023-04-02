using System;
using doob.Scripter.Shared;

namespace doob.Scripter.Module.Common
{

    public class CommonModule : IScripterModule
    {

        public GuidHelper Guid { get; } 
        public Sleep Sleep { get; }
        public Random Random { get; }


        public CommonModule(IScriptEngine scriptEngine)
        {
            Guid = new GuidHelper();
            Sleep = new Sleep();
            Random = new Random();
        }
    }

    public class GuidHelper
    {
        public Guid Parse(string guid) => Guid.Parse(guid);
        public Guid New() => Guid.NewGuid();
        public Guid Empty() => Guid.Empty;
    }

    public class Sleep
    {
        public void Milliseconds(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void Seconds(int seconds)
        {
            System.Threading.Thread.Sleep(seconds * 1000);
        }
        public void Minutes(int minutes)
        {
            System.Threading.Thread.Sleep(minutes * 60 * 1000);
        }
    }
}
