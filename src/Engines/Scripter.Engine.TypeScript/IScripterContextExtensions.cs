using System;
using doob.Scripter.Engine.Javascript;
using doob.Scripter.Shared;

namespace doob.Scripter.Engine.TypeScript
{
    public static class IScripterContextExtensions
    {

        public static IScripterContext AddTypeScriptEngine(this IScripterContext scripterContext)
        {
            return AddTypeScriptEngine(scripterContext, new TypeScriptEngineOptions());
        }

        public static IScripterContext AddTypeScriptEngine(this IScripterContext scripterContext, Action<TypeScriptEngineOptions> options)
        {
            var opts = new TypeScriptEngineOptions();
            options(opts);

            return scripterContext.AddScripterEngine<TypeScriptEngine, TypeScriptEngineOptions>(opts);
           
        }

        private static IScripterContext AddTypeScriptEngine(this IScripterContext scripterContext, TypeScriptEngineOptions options)
        {
            return scripterContext.AddScripterEngine<TypeScriptEngine, TypeScriptEngineOptions>(options);

        }

    }
}
