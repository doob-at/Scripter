using System;

namespace doob.Scripter.Shared
{
    public class CachedEngineEntry {
        public Type EngineType { get; set; }
        public Func<IServiceProvider, IScriptEngine>? Factory { get; set; }
        public IScriptEngineOptions? Options { get; set; }

        public CachedEngineEntry(Type engineType, Func<IServiceProvider, IScriptEngine>? factory, IScriptEngineOptions options)
        {
            EngineType = engineType;
            Factory = factory;
            Options = options;
        }
    };
}