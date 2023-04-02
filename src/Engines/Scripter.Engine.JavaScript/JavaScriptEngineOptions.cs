using System;
using System.Collections.Generic;
using System.Reflection;
using doob.Scripter.Shared;
using Jint;
using Jint.Runtime.Debugger;

namespace doob.Scripter.Engine.Javascript
{
    public class JavaScriptEngineOptions: IScriptEngineOptions
    {
        internal Options JintOptions { get; }
        public List<Type> AllowedExtensionMethods = new();

        public JavaScriptEngineOptions()
        {
            JintOptions = new Options()
                .CatchClrExceptions()
                .DebugMode()
                .AllowOperatorOverloading()
                .DebuggerStatementHandling(DebuggerStatementHandling.Script);
        }

        public JavaScriptEngineOptions AddExtensionMethods<T>()
        {
            return AddExtensionMethods(typeof(T));
        }

        public JavaScriptEngineOptions AddExtensionMethods(params Type[] types)
        {
            JintOptions.AddExtensionMethods(types);
            AllowedExtensionMethods.AddRange(types);
            return this;
        }

        public JavaScriptEngineOptions AllowAssemblies(params Assembly[] assemblies)
        {
            JintOptions.AllowClr(assemblies);
            return this;
        }

        public JavaScriptEngineOptions AllowCurrentDomainAssemblies()
        {
            return AllowAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        }

    }
}
