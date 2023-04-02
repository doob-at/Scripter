using System.Collections.Generic;

namespace doob.Scripter.Shared
{
    public interface IScripterEngineRegistry
    {
        CachedEngineEntry? GetRegisteredEngine(string name);
        IEnumerable<CachedEngineEntry> GetRegisteredEngines();
    }
}