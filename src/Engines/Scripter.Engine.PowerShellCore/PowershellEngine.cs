using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using doob.Reflectensions;
using doob.Reflectensions.ExtensionMethods;
using doob.Scripter.Shared;
using Microsoft.PowerShell.Commands;
using Namotion.Reflection;

namespace doob.Scripter.Engine.Powershell
{
    public class PowerShellCoreEngine: IScriptEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IScripter _scripter;
        public Func<string, string> CompileScript => s => s;
        public bool NeedsCompiledScript => false;
       

        private PsRunspace _psEngine;
        private Dictionary<Type, Func<object>> ProvidedTypeFactories = new Dictionary<Type, Func<object>>();
        private List<string> UseTaggedModules = new List<string>();
        
        private ScripterModulesProvider _scripterModulesProvider;

        public PowerShellCoreEngine(IServiceProvider serviceProvider, IScripter scripter)
        {
            _serviceProvider = serviceProvider;
            _scripter = scripter;
            _psEngine = new PsRunspace();

            _scripterModulesProvider =
                new ScripterModulesProvider(scripter, _serviceProvider, this, ProvidedTypeFactories, UseTaggedModules);

            _psEngine.SetVariable("ModulesProvider", _scripterModulesProvider);
        }
        
        //private void Initialize()
        //{
        //    _scripterModulesProvider =
        //        new ScripterModulesProvider(_serviceProvider, this, ProvidedTypeFactories, UseTaggedModules);

        //    _psEngine.SetVariable("ModulesProvider", _scripterModulesProvider);

            
        //}

        public void Stop()
        {
            
            _psEngine.Stop();
        }

        public object? ConvertToDefaultObject(object? value)
        {
            var json = JsonStringify(value);
            return JsonParse(json);
        }

        public object JsonParse(string? json)
        {
            return JsonObject.ConvertFromJson(json, false, null, out var err);

            //return Json.Converter.ToObject<ExpandoObject>(json);
        }

        public string JsonStringify(object? value)
        {
            var _json_context = new JsonObject.ConvertToJsonContext(maxDepth: 99, enumsAsStrings: true, compressOutput: true);
            string json_result = JsonObject.ConvertToJson(value, _json_context);
            return json_result; // Json.Converter.ToJson(value);
        }

        public void AddModuleParameterInstance(Type type, Func<object> factory)
        {
            ProvidedTypeFactories[type] = factory;
        }

        public void AddTaggedModules(params string[] tags)
        {
            UseTaggedModules.AddRange(tags);
        }

        public T? GetModuleState<T>()
        {
            var type = typeof(T);
            if (_scripterModulesProvider._instantiatedModules.ContainsKey(type))
            {
                return (T)_scripterModulesProvider._instantiatedModules[type];
            }

            return default;
        }

        public ScripterFunction? GetFunction(string name)
        {

            var fD = _psEngine.Invoke($"$fD = Get-Command -Name '{name}' -ErrorAction SilentlyContinue | Select-Object -First 1; $fD | Select-Object Name").FirstOrDefault();
            if (fD != null)
            {
                
                var parameters = _psEngine.Invoke(
                    "$fD.Parameters.GetEnumerator() | Select-Object Key, @{Name=\"Type\"; Expression={$_.Value.ParameterType}}");
                var dict = new Dictionary<string, Type>();
                foreach (var psObject in parameters)
                {
                    var k = psObject.Properties["Key"].Value.ToString();
                    
                    dict.Add(k, typeof(UnknownType));
                }
                
                return new ScripterFunction(fD.Properties["Name"].Value.ToString(), dict, this);
            }
            return null;

        }

        public object InvokeFunction(string name, params object[] args)
        {
            return _psEngine.ExecuteFunction(name, args);
        }

        public void SetValue(string name, object value)
        {
            _psEngine.SetVariable(name, value);
        }

        public string GetValueAsJson(string name)
        {
            var value = _psEngine.GetVariable(name);
            return JsonStringify(value);
        }

        public T? GetValue<T>(string name)
        {
            var value = _psEngine.GetVariable(name);
            if (LanguagePrimitives.TryConvertTo(value, out T val))
            {
                return val;
            }

            if (value.Reflect().TryTo(typeof(T), out var val2))
            {
                return (T)val2!;
            }
            var json = JsonStringify(value);

            return Json.Converter.ToObject<T>(json);


        }

        public object GetValue(string name)
        {
            return _psEngine.GetVariable(name);
        }

        public Task<object> ExecuteAsync(string script)
        {
            object res = _psEngine.Invoke(script);
            return Task.FromResult(res);
        }

        public string? Invoke(string script)
        {
            
            var results = _psEngine.Invoke(script).ToList();

            

            if (results.Count == 0)
            {
                return null;
            }

            if (results.Count == 1)
            {
                return Json.Converter.ToJson(results[0]);
            }

            return Json.Converter.ToJson(results);

        }

        public void Dispose()
        {
            _psEngine?.Dispose();
        }
    }
}
