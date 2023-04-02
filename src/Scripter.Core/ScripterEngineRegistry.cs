using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doob.Scripter.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Core
{
    public class ScripterEngineRegistry : IScripterEngineRegistry
    {
        private ConcurrentDictionary<string, CachedEngineEntry> RegisteredEngines { get; } = new(StringComparer.CurrentCultureIgnoreCase);
        private List<string> _registered = new();


        internal void RegisterEngine(Type engineType, Func<IServiceProvider, IScriptEngine> factory)
        {

            if (_registered.Contains(engineType.FullName!))
                return;

            var language = TrimEnd(engineType.Name, "Engine");

            RegisteredEngines.TryAdd(language, new CachedEngineEntry(engineType, factory, null));
            _registered.Add(engineType.FullName!);
           
        }

        internal void RegisterEngine(Type engineType, IScriptEngineOptions options)
        {

            if (_registered.Contains(engineType.FullName!))
                return;

            var language = TrimEnd(engineType.Name, "Engine");

            RegisteredEngines.TryAdd(language, new CachedEngineEntry(engineType, null, options));
            _registered.Add(engineType.FullName!);

        }

        public CachedEngineEntry? GetRegisteredEngine(string name)
        {
            return RegisteredEngines.TryGetValue(name, out var eng) ? eng : null;
        }
       
        public IEnumerable<CachedEngineEntry> GetRegisteredEngines()
        {
            return RegisteredEngines.Values;
        }

        private static string TrimEnd(string value, string trim)
        {

            if (value.EndsWith(trim))
            {
                value = value.Substring(0, value.Length - trim.Length);
            }

            return value;
        }
    }
}
