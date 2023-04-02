using System;
using System.Collections.Generic;
using doob.Scripter.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Engine.Powershell
{
    public class ScripterModulesProvider
    {
        private readonly IScripter _scripter;
        private readonly IServiceProvider _serviceProvider;
        private readonly IScriptEngine _scriptEngine;
        private readonly Dictionary<Type, Func<object>> _providedTypeFactories;
        private readonly List<string> _useTaggedModules;
        internal Dictionary<Type, object> _instantiatedModules = new Dictionary<Type, object>();

        public ScripterModulesProvider(IScripter scripter, IServiceProvider serviceProvider, IScriptEngine scriptEngine,
            Dictionary<Type, Func<object>> providedTypeFactories, List<string> useTaggedModules)
        {
            _scripter = scripter;
            _serviceProvider = serviceProvider;
            _scriptEngine = scriptEngine;
            _providedTypeFactories = providedTypeFactories;
            _useTaggedModules = useTaggedModules;
        }

        public object GetModule(string moduleName)
        {
            //var scripterModuleRegistry = _serviceProvider.GetRequiredService<IScripterModuleRegistry>();
            var inst = _scripter.ModuleRegistry.BuildModuleInstance(moduleName, _serviceProvider, _scriptEngine, _providedTypeFactories, _useTaggedModules);
            _instantiatedModules[inst.GetType()] = inst;
            return inst;
        }
    }
}
