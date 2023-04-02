using System;
using doob.Scripter.Shared;

namespace doob.Scripter.Engine.Javascript
{
    public static class IScripterContextExtensions
    {
        public static IScripterContext AddJavaScriptEngine(this IScripterContext services)
        {
            return AddJavaScriptEngine(services,  new JavaScriptEngineOptions());
        }

        public static IScripterContext AddJavaScriptEngine(this IScripterContext services, Action<JavaScriptEngineOptions> options)
        {
            var opts = new JavaScriptEngineOptions();
            options(opts);

            return AddJavaScriptEngine(services, opts);
        }

        private static IScripterContext AddJavaScriptEngine(this IScripterContext services, JavaScriptEngineOptions options)
        {
            
            return services.AddScripterEngine<JavaScriptEngine, JavaScriptEngineOptions>(options);
        }
    }
}
