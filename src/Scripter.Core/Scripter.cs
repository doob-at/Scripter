using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doob.Scripter.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Core
{
    public class Scripter : IScripter
    {
        private readonly IServiceProvider _serviceProvider;

        public IScripterModuleRegistry ModuleRegistry { get; }
        public IScripterEngineRegistry EngineRegistry { get; }

        internal Scripter(IServiceProvider serviceProvider, ScripterModuleRegistry moduleRegistry, ScripterEngineRegistry engineRegistry)
        {
            _serviceProvider = serviceProvider;
            ModuleRegistry = moduleRegistry;
            EngineRegistry = engineRegistry;
        }


        public IScriptEngine? GetScriptEngine(string name)
        {
            var engineEntry = EngineRegistry.GetRegisteredEngine(name);

            if (engineEntry == null)
                throw new Exception($"Engine '{name}' not found!");

            if (engineEntry.Factory != null)
            {
                return engineEntry.Factory(_serviceProvider);
            }

            var constructor = engineEntry.EngineType.GetConstructors().FirstOrDefault();
            if (constructor != null)
            {
                var parameterInfos = constructor.GetParameters();
                if (parameterInfos.Length == 0)
                {
                    return (IScriptEngine)ActivatorUtilities.CreateInstance(_serviceProvider, engineEntry.EngineType);
                }
                else
                {
                    var instanceDictionary = new Dictionary<Type, Func<object?>>();
                    instanceDictionary[typeof(IScripter)] = () => this;
                    instanceDictionary[typeof(IScriptEngineOptions)] = () => engineEntry.Options;

                    var parameters = BuildConstructorParameters(parameterInfos, _serviceProvider, instanceDictionary);
                    return (IScriptEngine)Activator.CreateInstance(engineEntry.EngineType, parameters )!;
                }
            }

            return null;
            //if (engineEntry.Options != null)
            //{
            //    return (IScriptEngine)ActivatorUtilities.CreateInstance(_serviceProvider, engineEntry.EngineType, engineEntry.Options);
            //}
            //else
            //{
            //    return (IScriptEngine)ActivatorUtilities.CreateInstance(_serviceProvider, engineEntry.EngineType);
            //}



        }

        private object?[] BuildConstructorParameters(ParameterInfo[] parameterInfos, IServiceProvider serviceProvider, Dictionary<Type, Func<object?>> instances)
        {
            var parameterInstances = new List<object?>();
            foreach (var parameterInfo in parameterInfos)
            {
                if (instances.TryGetValue(parameterInfo.ParameterType, out var instance))
                {
                    parameterInstances.Add(instance());
                }
                else
                {
                    var matching = instances.Keys.FirstOrDefault(t => t.IsAssignableFrom(parameterInfo.ParameterType));
                    if (matching != null)
                    {
                        parameterInstances.Add(instances[matching]());
                    }
                    else
                    {
                        parameterInstances.Add(serviceProvider.GetService(parameterInfo.ParameterType)!);
                    }
                    
                }
            }

            return parameterInstances.ToArray();
        }

    }
}
