using System;

namespace doob.Scripter.Shared
{
    public class ScripterModuleAttribute : Attribute
    {
        public string? Name { get; set; }

        public string[] Tags { get; set; }
        
        public ScripterModuleAttribute(params string[] tags )
        {
            Tags = tags;
        }
    }
}
