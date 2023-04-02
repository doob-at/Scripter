using System;
using System.Collections.Generic;

namespace doob.Scripter.Shared
{
    public interface IScripterModuleDefinition
    {
        string Name { get; }
        List<string> Tags { get; }
        Type ModuleType { get; }
    }
}
