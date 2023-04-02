using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doob.Scripter;
using doob.Scripter.Core;
using doob.Scripter.Engine.Javascript;
using doob.Scripter.Engine.TypeScript;
using doob.Scripter.Module.Common;
using doob.Scripter.Module.Http;
using doob.Scripter.Shared;
using Jint;
using Jint.Native;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TypeScriptTests
{
    public class UnitTest1
    {
        private IServiceProvider ServiceProvider { get; }
        private readonly ITestOutputHelper _output;
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;

           
            var sc = new ServiceCollection();
            sc.AddScripter(options => options
                .AddJavaScriptEngine()
                .AddTypeScriptEngine()
                .AddScripterModule<HttpModule>()
                .AddScripterModule<CommonModule>()
            );


            ServiceProvider = sc.BuildServiceProvider();
        }

        [Fact]
        public async Task Test1()
        {
            var tsEngine = ServiceProvider.GetRequiredService<IScripter>().GetScriptEngine("TypeScript");

            var tsScript = @"
const a = 1;
const b = 2;
export const c = a + b;
";

            var jsScript = tsEngine.CompileScript(tsScript);

            await tsEngine.ExecuteAsync(jsScript);

            var result = tsEngine.GetValue<int>("c");
            Assert.Equal(3, result);

        }

        [Fact]
        public async Task TryToGetFunctionAndInvokeIt()
        {
            var tsEngine = ServiceProvider.GetRequiredService<IScripter>().GetScriptEngine("TypeScript");

            var tsScript = @"

export function myFunc(name: string, age: number) {
    return `${name}:${age}`
}

";

            var jsScript = tsEngine.CompileScript(tsScript);

            await tsEngine.ExecuteAsync(jsScript);

            var func = tsEngine.GetFunction("myFunc");
            var f1 = func.Invoke("Bernhard", 41);
            var t = tsEngine.InvokeFunction("myFunc", "Bernhard", 41);
           

        }

    }
}

