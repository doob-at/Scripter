using System;

namespace doob.Scripter.Shared
{
    public interface IScripterContext
    {
        IScripterContext AddScripterEngine<TEngine>() where TEngine : class, IScriptEngine;

        IScripterContext AddScripterEngine(Type engineType, IScriptEngineOptions? options);

        IScripterContext AddScripterEngine<TEngine>(Func<IServiceProvider, TEngine> factory) where TEngine : class, IScriptEngine;
        IScripterContext AddScripterEngine(Type engineType, Func<IServiceProvider, IScriptEngine> factory);
        
        IScripterContext AddScripterEngine<TEngine, TOptions>(TOptions options) where TEngine : class, IScriptEngine<TOptions> where TOptions : class, IScriptEngineOptions;

        IScripterContext AddScripterModule<TModule>() where TModule : class, IScripterModule;
        IScripterContext AddScripterModule(Type moduleType);
    }
}
