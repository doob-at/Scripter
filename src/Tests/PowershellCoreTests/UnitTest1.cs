using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using doob.Reflectensions;
using doob.Scripter;
using doob.Scripter.Core;
using doob.Scripter.Engine.Powershell;
using doob.Scripter.Engine.Powershell.JsonConverter;

using doob.Scripter.Module.Http;
using doob.Scripter.Shared;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace PowershellCoreTests
{
    public class UnitTest1
    {
        private IServiceProvider ServiceProvider { get; }
        private readonly ITestOutputHelper _output;
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;

            Json.Converter.RegisterJsonConverter<PSObjectJsonConverter>();
            Json.Converter.JsonSerializer.Error += (sender, args) =>
            {
                args.ErrorContext.Handled = true;
            };

            var sc = new ServiceCollection();
            sc.AddScripter(options => options
                .AddPowerShellCoreEngine()
                .AddScripterModule<HttpModule>()
            );


            ServiceProvider = sc.BuildServiceProvider();
        }

        [Fact]
        public async Task Test1()
        {
            var psEngine = ServiceProvider.GetRequiredService<IScripter>().GetScriptEngine("PowerShellCore");

            var psScript = "$dt = get-date";

            await psEngine.ExecuteAsync(psScript);
            var dt = psEngine.GetValue<DateTime>("dt");

            Assert.Equal(dt.Minute, DateTime.Now.Minute);
        }

        [Fact]
        public async Task PsObjectConverterTest()
        {

            var psEngine = ServiceProvider.GetRequiredService<IScripter>().GetScriptEngine("PowerShellCore");

            var psScript = @"
$res = $PsVersionTable.PSVersion
";

            await psEngine.ExecuteAsync(psScript);
            var res = psEngine.GetValue<Version>("res");

            Assert.Equal(7, res.Major);
            var json = Json.Converter.ToJson(res, true);

            _output.WriteLine(json);

        }


        [Fact]
        public async Task GetProcessTest()
        {
            var proc = Process.GetCurrentProcess();
            var psEngine = ServiceProvider.GetRequiredService<IScripter>().GetScriptEngine("PowerShellCore");

            var psScript = @"
$res = [System.Diagnostics.Process]::GetCurrentProcess()
";

            await psEngine.ExecuteAsync(psScript);
            //var j = psEngine.GetValueAsJson("res");
            var res = psEngine.GetValue<Process>("res");

            Assert.Equal(proc.Id, res.Id);
           
        }


        [Fact]
        public async Task TryToGetFunctionAndInvokeIt()
        {
            var psEngine = ServiceProvider.GetRequiredService<IScripter>().GetScriptEngine("PowerShellCore");

            var psScript = @"

function ExecuteResponse([string]$name, [int]$age) {
    return ""$($name):$($age)""
}
";

           

            await psEngine.ExecuteAsync(psScript);

            var func = psEngine.GetFunction("ExecuteResponse");
            if (func != null)
            {
                var returnValue = func.Invoke("Bernhard", 41);
                //var returnValue = tsEngine.InvokeFunction("ExecuteResponse", "Bernhard", 41);

            }

        }

    }
}
