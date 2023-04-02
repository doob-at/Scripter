using System;
using System.Collections.Generic;
using doob.Scripter.Shared;

namespace doob.Scripter.Core
{
    public class ScripterModuleDefinition: IScripterModuleDefinition
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Type ModuleType { get; }

        public ScripterModuleDefinition(string name, Type moduleType)
        {
            Name = name;
            ModuleType = moduleType;
        }
    }
}
