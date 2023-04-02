using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doob.Scripter.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Core
{
    public class ScripterModuleRegistry : IScripterModuleRegistry
    {
        private ConcurrentDictionary<string, IScripterModuleDefinition> RegisteredModules { get; } = new ConcurrentDictionary<string, IScripterModuleDefinition>();
        private List<string> _registered = new List<string>();


        internal void RegisterModule(Type moduleType)
        {

            if (_registered.Contains(moduleType.FullName!))
                return;

            var moduleAttribute = moduleType.GetCustomAttribute<ScripterModuleAttribute>();
           
            var name = moduleAttribute?.Name ?? TrimEnd(moduleType.Name, "Module");
            
            var moduleDefinition = new ScripterModuleDefinition(name, moduleType);
            moduleDefinition.Tags = moduleAttribute?.Tags?.Select(t => t.ToLower()).ToList() ?? new List<string>();

            RegisteredModules.TryAdd(name, moduleDefinition);

            _registered.Add(moduleType.FullName!);
           
        }
        
        public IScripterModule BuildModuleInstance(string name, IServiceProvider serviceProvider,
            IScriptEngine currentScriptEngine, Dictionary<Type, Func<object>>? instanceDictionary = null,
            List<string>? useTaggedModules = null)
        {

            if (!RegisteredModules.TryGetValue(name, out var module))
            {
                throw new Exception($"A ScripterModule with name '{name}' is not registered!");
            }

            if (useTaggedModules != null)
            {
                if (module.Tags?.Any() == true)
                {
                    var hasAllowedTag = useTaggedModules.Any(tm => module.Tags.Contains(tm.ToLower()));
                    if (!hasAllowedTag)
                    {
                        throw new Exception($"Module '{name}' is not available in this ScriptingContext");
                    }
                }
            }

            return BuildSingleModuleInstance(module, serviceProvider, currentScriptEngine, instanceDictionary);

        }

        public IScripterModule BuildSingleModuleInstance(IScripterModuleDefinition module, IServiceProvider serviceProvider,
            IScriptEngine currentScriptEngine, Dictionary<Type, Func<object>>? instanceDictionary = null)
        {

            var constructor = module.ModuleType.GetConstructors().FirstOrDefault();
            if(constructor == null)
            {
                return (IScripterModule)ActivatorUtilities.CreateInstance(serviceProvider, module.ModuleType
                );
            }

            var parameterInfos = constructor.GetParameters();
            if (parameterInfos.Length == 0)
            {
                return (IScripterModule)ActivatorUtilities.CreateInstance(serviceProvider, module.ModuleType);
            }
            else
            {
                instanceDictionary ??= new Dictionary<Type, Func<object>>();
                instanceDictionary[typeof(IScriptEngine)] = () => currentScriptEngine;


                return (IScripterModule)Activator.CreateInstance(module.ModuleType, BuildConstructorParameters(parameterInfos, serviceProvider, instanceDictionary))!;
            }

        }

        public IEnumerable<IScripterModuleDefinition> GetRegisteredModuleDefinitions()
        {
            return RegisteredModules.Values;
        }


        private object[] BuildConstructorParameters(ParameterInfo[] parameterInfos, IServiceProvider serviceProvider, Dictionary<Type, Func<object>> instances)
        {
            var parameterInstances = new List<object>();
            foreach (var parameterInfo in parameterInfos)
            {
                if(instances.TryGetValue(parameterInfo.ParameterType, out var instance))
                {
                    parameterInstances.Add(instance());
                }
                else
                {
                    parameterInstances.Add(serviceProvider.GetService(parameterInfo.ParameterType)!);
                }
            }

            return parameterInstances.ToArray();
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
