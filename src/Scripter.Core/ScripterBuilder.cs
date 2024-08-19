using System;
using System.Collections.Generic;
using doob.Scripter.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Core
{
    public class ScripterBuilder : IScripterContext
    {
        internal readonly ScripterModuleRegistry _moduleRegistry;
        internal readonly ScripterEngineRegistry _engineRegistry;

        internal ScripterBuilder(ScripterModuleRegistry moduleRegistry, ScripterEngineRegistry engineRegistry)
        {
            _moduleRegistry = moduleRegistry;
            _engineRegistry = engineRegistry;
        }

        public IScripterContext AddScripterEngine<TEngine>() where TEngine : class, IScriptEngine
        {
            return AddScripterEngine(typeof(TEngine));
        }

        public IScripterContext AddScripterEngine<TEngine, TOptions>(TOptions options) where TEngine : class, IScriptEngine<TOptions> where TOptions : class, IScriptEngineOptions
        {
            return AddScripterEngine(typeof(TEngine), options);
        }


        public IScripterContext AddScripterEngine(Type engineType, IScriptEngineOptions? options = null)
        {
            _engineRegistry.RegisterEngine(engineType, options);
            return this;
        }

        public IScripterContext AddScripterEngine<TEngine>(Func<IServiceProvider, TEngine> factory) where TEngine : class, IScriptEngine
        {
            return AddScripterEngine(typeof(TEngine), factory);
        }

        public IScripterContext AddScripterEngine(Type engineType, Func<IServiceProvider, IScriptEngine> factory)
        {
            _engineRegistry.RegisterEngine(engineType, factory);
            return this;
        }

       

        public IScripterContext AddScripterModule<TModule>() where TModule : class, IScripterModule
        {
            return AddScripterModule(typeof(TModule));
        }
        public IScripterContext AddScripterModule(Type moduleType)
        {
            _moduleRegistry.RegisterModule(moduleType);
            return this;
        }

    }
}
